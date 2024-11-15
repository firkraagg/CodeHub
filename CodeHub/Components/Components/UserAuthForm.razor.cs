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

    private bool showAlert = false;
    private string alertMessage;

    public async Task HandleFormSubmitAsync(EditContext editContext)
    {
        if (FormName == "RegisterForm")
        {
            var newUser = (User)editContext.Model;
            newUser.CreatedAt = DateTime.Now;
            newUser.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(newUser.Password);
            userService.AddUser(newUser);
            NavigationManager.NavigateTo("/");
        }
        else if (FormName == "LoginForm")
        {
            var storedUser = userService.FindByEmail(user.Email);
            if (storedUser == null)
            {
                alertMessage = "Používateľ s touto e-mailovou adresou neexistuje. Skontrolujte zadaný e-mail alebo sa zaregistrujte.";
                showAlert = true;
            }
            else
            {
                bool isPasswordValid = BCrypt.Net.BCrypt.EnhancedVerify(user.Password, storedUser.Password);
                if (isPasswordValid)
                {
                    NavigationManager.NavigateTo("/");
                }
                else
                {
                    alertMessage = "Zadané heslo je nesprávne. Skúste to znova.";
                    showAlert = true;
                }
            }
        }
    }
}