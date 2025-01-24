using CodeHub.Data.Entities;
using CodeHub.Data.Models;
using CodeHub.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace CodeHub.Components.Pages;

public partial class UserProfile
{
    private User? _user;
    private bool _showModal = false;

    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var userId = ((CustomAuthStateProvider)AuthenticationStateProvider).GetLoggedInUserId();
        if (!string.IsNullOrEmpty(userId))
        {
            _user = await UserService.GetUserByIdAsync(userId);
        }
    }

    public async Task UpdateUser()
    {
        
    }

    public async Task DeleteUser()
    {
        _showModal = true;
        StateHasChanged();
    }

    public void CloseModal()
    {
        _showModal = false;
        StateHasChanged();
    }
}