using CodeHub.Data.Entities;
using CodeHub.Data.Models;
using CodeHub.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;

namespace CodeHub.Components.Pages;

public partial class UserProfile
{
    private User? _user;
    private string? _uploadedImage;
    private bool _showModal = false;
    private bool _showEditProfile = false;
    private bool _showChangePassword = false;
    private bool _showDeleteModal = false;
    private bool _showAlert;
    private string _alertMessage = String.Empty;
    private string _alertColor = "alert-danger";

    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;
    [SupplyParameterFromForm] private EditModel em { get; set; } = new();
    [SupplyParameterFromForm] private ChangePasswordModel cpm { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        var userId = ((CustomAuthStateProvider)AuthenticationStateProvider).GetLoggedInUserId();
        if (!string.IsNullOrEmpty(userId))
        {
            _user = await UserService.GetUserByIdAsync(userId);
            em.Username = _user!.Username;
            em.Email = _user.Email;
        }
    }

    public async Task HandleEditProfileFormSubmitAsync(EditContext editContext)
    {
        _showAlert = false;
        var userUsername = await UserService.FindByUsernameAsync(em.Username);
        var userEmail = await UserService.FindByEmailAsync(em.Email);
        if (userUsername != null && userUsername.Username != _user!.Username)
        {
            _alertColor = "alert-danger";
            _alertMessage = "Používateľ s touto prezývkou už existuje.";
            _showAlert = true;
        } else if (userEmail != null && userEmail.Email != _user!.Email)
        {
            _alertColor = "alert-danger";
            _alertMessage = "Používateľ s touto e-mailovou adresou už existuje.";
            _showAlert = true;
        } else
        {
            _user!.Username = em.Username;
            _user.Email = em.Email;
            _user.ProfileImage = _uploadedImage != null ? Convert.FromBase64String(_uploadedImage.Split(",")[1]) : _user.ProfileImage;
            await UserService.EditUserAsync(_user!);
            _alertColor = "alert-success";
            _alertMessage = "Údaje boli úspešne zmenené.";
            _showAlert = true;
            _uploadedImage = null;
        }
    }

    public async Task HandleChangePasswordFormSubmitAsync(EditContext editContext)
    {
        _showAlert = false;
        bool isPasswordValid = BCrypt.Net.BCrypt.EnhancedVerify(cpm.Password, _user!.PasswordHash);
        if (!isPasswordValid)
        {
            _alertColor = "alert-danger";
            _alertMessage = "Zadané staré heslo je nesprávne.";
            _showAlert = true;
        } else if (BCrypt.Net.BCrypt.EnhancedVerify(cpm.NewPassword, _user!.PasswordHash))
        {
            _alertColor = "alert-danger";
            _alertMessage = "Vaše nové heslo nemôže byť rovnaké ako vaše staré heslo.";
            _showAlert = true;
        }
        else
        {
            _user!.PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(cpm.NewPassword);
            await UserService.EditUserAsync(_user);
            _alertColor = "alert-success";
            _alertMessage = "Heslo bolo zmenené.";
            _showAlert = true;
            cpm.Password = string.Empty;
            cpm.NewPassword = string.Empty;
            cpm.ConfirmPassword = string.Empty;
        }
    }

    public async Task HandleFileSelected(InputFileChangeEventArgs e)
    {
        var file = e.File;
        if (file != null)
        {
            var buffer = new byte[file.Size];
            await file.OpenReadStream().ReadAsync(buffer);
            _uploadedImage = $"data:{file.ContentType};base64,{Convert.ToBase64String(buffer)}";
            StateHasChanged();
        }
    }
    
    public void ShowEditProfile()
    {
        _showAlert = false;
        _showChangePassword = false;
        _showEditProfile = true;
        StateHasChanged();
    }

    public void ShowChangePassword()
    {
        _showEditProfile = false;
        _showChangePassword = true;
        StateHasChanged();
    }

    public void ShowDeleteModal()
    {
        _showDeleteModal = true;
        StateHasChanged();
    }

    public void CloseDeleteModal()
    {
        _showDeleteModal = false;
        StateHasChanged();
    }

    public async Task UpdateUser()
    {
        
    }
    
    public async Task DeleteUser()
    {
        await UserService.DeleteUserAsync(_user!);
        await UserService.LogoutUserAsync();
        NavigationManager.NavigateTo("/", true);
        CloseDeleteModal();
        StateHasChanged();
    }

    public void CloseModal()
    {
        _showModal = false;
        StateHasChanged();
    }
}