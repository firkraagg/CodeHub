﻿using CodeHub.Data.Entities;
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
    private int _numberOfPassedTests;
    private bool _allTestsPassed;
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
        _output = "Kód ešte nebol spustený. Kliknite na \"Skontroluj\" pre zobrazenie výstupu kódu.";
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

    private void HandleResultReceived(string output, int NumberOfPassedTests, bool allTestsPassed)
    {
        InvokeAsync(() =>
        {
            _output = output;
            _allTestsPassed = allTestsPassed;
            _numberOfPassedTests = NumberOfPassedTests;
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
        var numberOfTestCases = 0;
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
            numberOfTestCases = testCases.Count;

            await rabbitMqProducer.SendToRabbitMq(codeToSend, languageName, testCases, isEvaluation);
            await _executionCompletion.Task;
        }
        catch (Exception e)
        {
            _noErrors = false;
            throw;
        }
        finally
        {
            _noErrors = !_output.ToLower().Contains("error") && !_output.ToLower().Contains("exception") && !_output.ToLower().Contains("failed") && !_output.ToLower().Contains("timed out")
                && !_output.ToLower().Contains("invalid") && !_output.ToLower().Contains("Časový limit") && _allTestsPassed;
            _isCheckLoading = false;
            _isSubmitLoading = false;
            _hasExecuted = true;

            var userId = ((CustomAuthStateProvider)AuthenticationStateProvider).GetLoggedInUserId();
            if (!string.IsNullOrEmpty(userId))
            {
                _user = await UserService.GetUserByIdAsync(userId);
            }

            if (isEvaluation)
            {
                var points = ((double)_numberOfPassedTests / numberOfTestCases) * _problem!.Points;
                var solvedProblem = new ProblemAttempt
                {
                    problemId = _problem.Id,
                    userId = int.Parse(userId),
                    AttemptedAt = DateTime.UtcNow.AddHours(2),
                    SourceCode = codeToSend,
                    PassedTestCases = _numberOfPassedTests,
                    Points = points,
                    IsSuccessful = _allTestsPassed
                };

                await SolvedProblemsService.AddSolvedProblemAsync(solvedProblem);
            }
        }
    }
}