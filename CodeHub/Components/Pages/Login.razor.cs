using System.Security.Claims;
using CodeHub.Data.Entities;
using CodeHub.Data.Models;
using CodeHub.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;

namespace CodeHub.Components.Pages;

public partial class Login
{
    [Inject] private UserService userService { get; set; }
    [Inject] private NavigationManager NavigationManager { get; set; }
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;
    [SupplyParameterFromForm] private LoginModel lm { get; set; } = new();

    private bool _showAlert;
    private string _alertMessage = String.Empty;
    private string _alertColor = "alert-danger";

    protected override void OnInitialized()
    {
        var uri = new Uri(NavigationManager.Uri);
        var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);

        if (query.TryGetValue("message", out var message) && message == "registered")
        {
            _alertMessage = "Registrácia prebehla úspešne! Teraz sa prihláste.";
            _alertColor = "alert-success";
            _showAlert = true;
        }
    }

    public async Task HandleFormSubmitAsync(EditContext editContext)
    {
        var storedUser = await userService.FindByEmailAsync(lm.Email);
        if (storedUser == null)
        {
            _alertMessage = "Používateľ s touto e-mailovou adresou neexistuje. Skontrolujte zadaný e-mail alebo sa zaregistrujte.";
            _showAlert = true;
        }
        else
        {
            bool isPasswordValid = BCrypt.Net.BCrypt.EnhancedVerify(lm.Password, storedUser.PasswordHash);
            if (isPasswordValid)
            {
                string token = userService.CreateToken(storedUser);
                await userService.LoginUser(storedUser);

                var claims = ((CustomAuthStateProvider)AuthenticationStateProvider).GetClaimsFromToken(token);
                ((CustomAuthStateProvider)AuthenticationStateProvider).TriggerAuthenticationStateChanged();


                NavigationManager.NavigateTo("/");
            }
            else
            {
                _alertMessage = "Zadané heslo je nesprávne. Skúste to znova.";
                _showAlert = true;
            }
        }
    }
}