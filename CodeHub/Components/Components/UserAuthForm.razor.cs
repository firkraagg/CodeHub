using Microsoft.AspNetCore.Components;
using CodeHub.Data.Models;
using CodeHub.Services;
using Microsoft.AspNetCore.Components.Forms;

namespace CodeHub.Components.Components;

public partial class UserAuthForm
{
    [Inject] private UserService userService { get; set; }
    [Inject] private NavigationManager NavigationManager { get; set; }

    [Parameter] public string Title { get; set; }
    [Parameter] public bool showNickname { get; set; }
    [Parameter] public bool ShowConfirmPassword { get; set; }
    [Parameter] public bool ShowPasswordLayout { get; set; }
    [Parameter] public bool ShowRememberMe { get; set; }
    [Parameter] public string ButtonLabel { get; set; }
    [Parameter] public string FormName { get; set; }

    [SupplyParameterFromForm] private User user { get; set; } = new User();

    public async Task HandleRegistrationAsync(EditContext editContext)
    {
        var newUser = (User)editContext.Model;
        newUser.CreatedAt = DateTime.Now;
        userService.AddUser(newUser);

        NavigationManager.NavigateTo("/");
    }
}