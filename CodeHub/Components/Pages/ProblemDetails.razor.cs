using CodeHub.Data.Entities;
using CodeHub.Data.Models;
using CodeHub.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CodeHub.Components.Pages;

public partial class ProblemDetails
{
    [Parameter] public int ProblemId { get; set; }

    private Problem? _problem;
    private List<Tag> _tags = new();
    private string _selectedLanguage = "java";
    private string _selectedTheme = "vs-dark";

    protected override async Task OnInitializedAsync()
    {
        _problem = await ProblemService.GetProblemByIdAsync(ProblemId);
        _tags = await TagService.GetTagsForProblemAsync(_problem.Id);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JS.InvokeVoidAsync("monacoInterop.initialize", "editorContainer", "csharp");
        }
    }

    private async Task GetEditorValue()
    {
        string code = await JS.InvokeAsync<string>("monacoInterop.getValue");
        Console.WriteLine($"Code from editor: {code}");
    }

    private async Task ChangeTheme(string theme)
    {
        _selectedTheme = theme;
        await JS.InvokeVoidAsync("monacoInterop.setTheme", _selectedTheme);
    }

    private async Task ChangeLanguage(string language)
    {
        _selectedLanguage = language switch
        {
            "Java" => "java",
            "C#" => "csharp",
            _ => "plaintext"
        };

        await JS.InvokeVoidAsync("monacoInterop.setLanguage", _selectedLanguage);
    }
}