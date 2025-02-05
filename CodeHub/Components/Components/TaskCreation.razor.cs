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
    private Problem _problem = new();
    private List<ProgrammingLanguage> _languages = new();
    private User? _user;
    private bool _showAlert;
    private string _alertColor = "";
    private string _alertMessage = "";
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var userId = ((CustomAuthStateProvider)AuthenticationStateProvider).GetLoggedInUserId();
        _languages = await ProgrammingLanguageService.GetProgrammingLanguagesAsync();
        if (!string.IsNullOrEmpty(userId))
        {
            _user = await UserService.GetUserByIdAsync(userId);
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