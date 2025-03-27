using CodeHub.Data.Entities;
using CodeHub.Data.Models;
using CodeHub.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace CodeHub.Components.Pages;

public partial class ProblemPage
{
    [Parameter] public int ProblemId { get; set; }
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;

    private Problem? _problem;
    private User? _user;
    private List<Tag> _tags = new();
    private List<ProgrammingLanguage> _languages = new();
    private ProgrammingLanguage _selectedLanguage = new();
    private List<ProblemHint> _hints = new();
    private List<ProblemConstraint> _constraints = new();
    private List<ProblemExample> _examples = new();
    private string _selectedTheme = "vs-dark";
    private string _userCode;
    private string _output;
    private bool _isSubmitLoading;
    private bool _isCheckLoading;
    private bool _noErrors;
    private TaskCompletionSource<bool> _executionCompletion = new();
    private bool _hasExecuted;
    private bool _isEditorInitialized = false;
    private CancellationTokenSource _cts = new();
    private bool _isLoading = true;

    protected override async Task OnInitializedAsync()
    {
        _problem = await ProblemService.GetProblemByIdAsync(ProblemId);
        _tags = await TagService.GetTagsForProblemAsync(_problem.Id);
        _languages = await ProgrammingLanguageService.GetProgrammingLanguagesAsync();
        if (_problem.LanguageID != 0)
        {
            _selectedLanguage = _languages.FirstOrDefault(lang => lang.Id == _problem.LanguageID);
        }
        else
        {
            _selectedLanguage = _languages.First();
        }

        _hints = await ProblemHintService.GetHintsForProblemAsync(_problem.Id);
        _constraints = await ProblemConstraintService.GetConstraintsForProblemAsync(_problem.Id);
        _examples = await ProblemExampleService.GetExamplesForProblemAsync(_problem.Id);

        RabbitMqProducerService.ResultReceived += HandleResultReceived;
        Task.Run(() => RabbitMqProducerService.ListenForResults());
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_problem == null || _isEditorInitialized) return;

        string defaultCode = string.IsNullOrEmpty(_problem.DefaultCode) ? "" : _problem.DefaultCode;
        await JS.InvokeVoidAsync("monacoInterop.initialize", "editorContainer", "java", defaultCode);
        _isEditorInitialized = true;

    }

    private void HandleResultReceived(string output)
    {
        InvokeAsync(() =>
        {
            _output = output;
            _executionCompletion.TrySetResult(true);
            StateHasChanged();
        });
    }

    public void Dispose()
    {
        RabbitMqProducerService.ResultReceived -= HandleResultReceived;
    }

    private async Task SendCodeToQueue(bool isEvaluation)
    {
        if (isEvaluation)
        {
            _isSubmitLoading = true;
        }
        else
        {
            _isCheckLoading = true;
        }

        _executionCompletion = new TaskCompletionSource<bool>();
        string codeToSend = await JS.InvokeAsync<string>("monacoInterop.getValue");
        try
        {
            if (string.IsNullOrEmpty(codeToSend))
            {
                _output = "Kód je prázdny";
                _noErrors = false;
                _isCheckLoading = false;
                _hasExecuted = true;
                return;
            }

            var rabbitMqProducer = new RabbitMqProducerService();
            var language = await ProgrammingLanguageService.GetProgrammingLanguageByIdAsync(_problem!.LanguageID);
            var languageName = language.Name;
            var testCases = await TestCaseService.GetTestCasesForProblemAsync(_problem.Id);

            await rabbitMqProducer.SendToRabbitMq(codeToSend, languageName, testCases, isEvaluation);
            await _executionCompletion.Task;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            _noErrors = !_output.ToLower().Contains("error") && !_output.ToLower().Contains("exception") && !_output.ToLower().Contains("failed") && !_output.ToLower().Contains("timed out")
                && !_output.ToLower().Contains("nebola") && !_output.ToLower().Contains("invalid"); ;
            _isCheckLoading = false;
            _isSubmitLoading = false;
            _hasExecuted = true;

            var userId = ((CustomAuthStateProvider)AuthenticationStateProvider).GetLoggedInUserId();
            if (!string.IsNullOrEmpty(userId))
            {
                _user = await UserService.GetUserByIdAsync(userId);
            }

            await SolvedProblemsService.DeleteSolvedProblemAsync(int.Parse(userId), _problem.Id);

            var solvedProblem = new ProblemAttempt
            {
                problemId = _problem.Id,
                userId = int.Parse(userId),
                AttemptedAt = DateTime.Now,
                SourceCode = codeToSend,
                IsSuccessful = _output == "Úloha bola vypracovaná správne."
            };

            await SolvedProblemsService.AddSolvedProblemAsync(solvedProblem);
        }
    }

    private async Task ChangeTheme(string theme)
    {
        if (_selectedTheme != theme)
        {
            _selectedTheme = theme;
            await JS.InvokeVoidAsync("monacoInterop.setTheme", _selectedTheme);
            StateHasChanged();
        }
    }
    private async Task ChangeLanguage(string name)
    {
        var language = _languages.FirstOrDefault(l => l.Name == name);
        _selectedLanguage = language ?? _languages.First();
        await JS.InvokeVoidAsync("monacoInterop.setLanguage", _selectedLanguage.MonacoName);
    }

    private async Task SetCode(string code)
    {
        _userCode = code;
        await JS.InvokeVoidAsync("monacoInterop.setValue", _userCode);
    }

}