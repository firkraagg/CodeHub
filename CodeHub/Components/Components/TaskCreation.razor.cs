using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using CodeHub.Data.Entities;
using CodeHub.Data.Models;
using CodeHub.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;

namespace CodeHub.Components.Components;

public partial class TaskCreation
{
    private Problem _problem = new Problem
    {
        Title = string.Empty,
        LanguageID = 0,
        Difficulty = 0,
        Description = string.Empty,
        RequiredInput = string.Empty,
        RequiredOutput = string.Empty,
        Constraints = string.Empty,
        Hints = string.Empty,
        Tags = new List<Tag>()
    };
    private List<ProgrammingLanguage> _languages = new();
    private List<Tag> _tags = new();
    private List<string> _selectedTags = new();
    private string _selectedTag = "";
    private User? _user;
    private bool _showAlert;
    private string _alertColor = "";
    private string _alertMessage = "";
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var userId = ((CustomAuthStateProvider)AuthenticationStateProvider).GetLoggedInUserId();
        _languages = await ProgrammingLanguageService.GetProgrammingLanguagesAsync();
        _tags = await TagService.GetTagsAsync();
        if (!string.IsNullOrEmpty(userId))
        {
            _user = await UserService.GetUserByIdAsync(userId);
        }
    }

    private void AddTag()
    {
        if (!string.IsNullOrEmpty(_selectedTag) && !_selectedTags.Contains(_selectedTag))
        {
            _selectedTags.Add(_selectedTag);
            _selectedTag = "";
        }
    }
    private void RemoveTag()
    {
        if (_selectedTags.Any())
        {
            _selectedTags.RemoveAt(_selectedTags.Count - 1);
        }
    }

    private void RemoveTag(string tag)
    {
        if (_selectedTags.Contains(tag))
        {
            _selectedTags.Remove(tag);
        }
    }

    public async Task HandleCreateProblemFormSubmitAsync()
    {
        var existingProblem = await ProblemService.GetProblemByName(_problem.Title);

        if (existingProblem != null)
        {
            _showAlert = true;
            _alertColor = "alert-danger";
            _alertMessage = "Tento názov úlohy už existuje";
            StateHasChanged();
            return;
        }

        if (_user != null)
        {
            var existingTags = await TagService.GetTagsAsync();
            foreach (var selectedTag in _selectedTags)
            {
                var tag = existingTags.FirstOrDefault(t => t.Name == selectedTag);
                if (tag != null)
                {
                    _problem.Tags.Add(tag);
                }
            }
            _problem.UserID = _user.Id;
            await ProblemService.CreateProblemAsync(_problem);
            StateHasChanged();
            NavigationManager.NavigateTo("#");
        }
    }

    public void CancelCreation()
    {
        NavigationManager.NavigateTo("#");
    }
}