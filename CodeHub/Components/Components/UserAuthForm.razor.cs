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
    private string alertColor = "alert-danger";

    protected override void OnInitialized()
    {
        var uri = new Uri(NavigationManager.Uri);
        var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);

        if (query.TryGetValue("message", out var message) && message == "registered")
        {
            alertMessage = "Boli ste úspešne zaregistrovaný! Teraz sa prihláste.";
            alertColor = "alert-success";
            showAlert = true;
        }
    }

    public async Task HandleFormSubmitAsync(EditContext editContext)
    {
        alertColor = "alert-danger";
        bool result =
            user.Password.Any(c => char.IsLetter(c)) &&
            user.Password.Any(c => char.IsDigit(c)) &&
            user.Password.Any(c => char.IsSymbol(c) || char.IsPunctuation(c));

        if (FormName == "RegisterForm")
        {
            if (user.Username.Equals(""))
            {
                alertMessage = "Zadajte používateľské meno.";
                showAlert = true;
            }
            else if (user.Email.Equals(""))
            {
                alertMessage = "Zadajte e-mailovú adresu.";
                showAlert = true;
            }
            else if (userService.FindByUsername(user.Username) != null)
            {
                alertMessage = "Používateľ s týmto používateľským menom už existuje. Zvoľte iné používateľské meno.";
                showAlert = true;
            }
            else if (userService.FindByEmail(user.Email) != null)
            {
                alertMessage = "Používateľ s touto e-mailovou adresou už existuje. Skontrolujte zadaný e-mail alebo sa prihláste.";
                showAlert = true;
            }
            else if (!result)
            {
                alertMessage = "Heslo musí obsahovať písmená, číslice a špeciálne znaky.";
                showAlert = true;
            }
            else if (user.Password.Length < 8)
            {
                alertMessage = "Heslo nesmie byť kratšie ako 8 znakov.";
                showAlert = true;
            }
            else
            {
                var newUser = (User)editContext.Model;
                newUser.CreatedAt = DateTime.Now;
                newUser.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(newUser.Password);
                userService.AddUser(newUser);
                NavigationManager.NavigateTo("/login?message=registered");
            }
            
        }
        else if (FormName == "LoginForm")
        {
            var storedUser = userService.FindByEmail(user.Email);
            if (user.Email.Equals(""))
            {
                alertMessage = "Zadajte e-mailovú adresu.";
                showAlert = true;
            }
            else if (user.Password.Equals(""))
            {
                alertMessage = "Zadajte heslo.";
                showAlert = true;
            }
            else if (storedUser == null)
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