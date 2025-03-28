using CodeHub.Data.Entities;
using CodeHub.Data.Models;
using CodeHub.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using System.Text;

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
        var storedUser = await userService.FindByUsernameAsync(lm.Nickname);

        if (storedUser != null)
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
                _alertColor = "alert-danger";
                _alertMessage = "Zadané heslo je nesprávne.";
                _showAlert = true;
            }
        }
        else
        {
            var ldapUser = await LdapService.AuthenticateUserAsync(lm.Nickname, lm.Password);
            if (ldapUser != null)
            {
                var entry = await LdapService.FindUser(lm.Nickname);
                var email = entry?.DirectoryAttributes["mail"].GetValue<string>();
                var group = entry?.DirectoryAttributes["physicalDeliveryOfficeName"].GetValue<string>();
                var username = entry?.DirectoryAttributes["uid"].GetValue<string>();
                var displayName = entry?.DirectoryAttributes["displayName"].GetValue<string>();

                var newUser = await UserService.CreateUserAsync(username!, email!, lm.Password, true, displayName!, group!);
                if (newUser != null)
                {
                    string token = userService.CreateToken(newUser);
                    await userService.LoginUser(newUser);

                    var claims = ((CustomAuthStateProvider)AuthenticationStateProvider).GetClaimsFromToken(token);
                    ((CustomAuthStateProvider)AuthenticationStateProvider).TriggerAuthenticationStateChanged();

                    NavigationManager.NavigateTo("/");
                }
            }
            else
            {
                _alertMessage = "Nesprávne prihlasovacie údaje.";
                _alertColor = "alert-danger";
                _showAlert = true;
            }
        }
    }
}