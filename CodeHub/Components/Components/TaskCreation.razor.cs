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
    private User? _user;
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var userId = ((CustomAuthStateProvider)AuthenticationStateProvider).GetLoggedInUserId();
        if (!string.IsNullOrEmpty(userId))
        {
            _user = await UserService.GetUserByIdAsync(userId);
        }
    }
    public async Task HandleCreateProblemFormSubmitAsync()
    {
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