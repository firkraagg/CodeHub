using CodeHub.Data.Entities;
using CodeHub.Data.Models;
using CodeHub.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json;

namespace CodeHub.Components.Pages;

public partial class ProblemDetails
{
    [Parameter] public int ProblemId { get; set; }

    private Problem? _problem;
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
    private bool _hasExecuted;

    protected override async Task OnInitializedAsync()
    {
        _problem = await ProblemService.GetProblemByIdAsync(ProblemId);
        _tags = await TagService.GetTagsForProblemAsync(_problem.Id);
        _languages = await ProgrammingLanguageService.GetProgrammingLanguagesAsync();
        _selectedLanguage = _languages.First();
        _hints = await ProblemHintService.GetHintsForProblemAsync(_problem.Id);
        _constraints = await ProblemConstraintService.GetConstraintsForProblemAsync(_problem.Id);
        _examples = await ProblemExampleService.GetExamplesForProblemAsync(_problem.Id);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JS.InvokeVoidAsync("monacoInterop.initialize", "editorContainer", "csharp", _userCode);
        }
    }

    private async Task GetEditorValue()
    {
        _isSubmitLoading = true;
        try
        {
            _userCode = await JS.InvokeAsync<string>("monacoInterop.getValue");
            string responseJson = await PistonService.ExecuteCodeAsync(_selectedLanguage.ApiName, _selectedLanguage.Version, _userCode);
            var response = JsonSerializer.Deserialize<PistonResponse>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (response?.Run != null)
            {
                _output = !string.IsNullOrEmpty(response.Run.Stdout)
                    ? response.Run.Stdout
                    : response.Run.Stderr;
            }
            else
            {
                _output = "No output available";
            }
        }
        catch (Exception)
        {
            _output = "Error: Failed to execute code";
            throw;
        }
        finally
        {
            _isSubmitLoading = false;
        }
    }

    private async Task CheckCodeAsync()
    {
        _isCheckLoading = true;
        try
        {
            _userCode = await JS.InvokeAsync<string>("monacoInterop.getValue");
            var executionService = new CodeExecutionService();
            string output = "";

            switch (_selectedLanguage.ApiName)
            {
                case "csharp.net":
                    output = await executionService.ExecuteCSharpCodeAsync(_userCode);
                    break;
                case "java":
                    output = await executionService.ExecuteJavaCodeAsync(_userCode);
                    break;
                default:
                    _output = "Chyba: Nepodporovaný jazyk";
                    break;
            }

            _hasExecuted = true;
            if (!string.IsNullOrEmpty(output))
            {
                _output = output;
                _noErrors = !output.Contains("error");
            }
            else
            {
                _output = "Žiadny výstup nie je k dispozícií";
            }
        }
        catch (Exception)
        {
            _output = "Chyba: Nepodarilo sa spustiť kód";
            throw;
        }
        finally
        {
            _isCheckLoading = false;
        }
    }


    private async Task ChangeTheme(string theme)
    {
        _selectedTheme = theme;
        await JS.InvokeVoidAsync("monacoInterop.setTheme", _selectedTheme);
        await SetCode(_userCode);
    }
    private async Task ChangeLanguage(string apiName)
    {
        var language = _languages.FirstOrDefault(l => l.ApiName == apiName);
        _selectedLanguage = language ?? _languages.First();
        await JS.InvokeVoidAsync("monacoInterop.setLanguage", _selectedLanguage.MonacoName);
    }

    private async Task SetCode(string code)
    {
        _userCode = code;
        await JS.InvokeVoidAsync("monacoInterop.setValue", _userCode);
    }

}