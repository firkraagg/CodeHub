using CodeHub.Data.Entities;
using System.Text.RegularExpressions;

namespace CodeHub.Components.Pages;

public partial class ManageAccounts
{
    private List<User> _users = new List<User>();
    private int _showUsersCount = 10;
    private int _skipUsersCount = 0;
    private User _editingUser = new User();
    private bool _showModal = false;
    private bool _showSuccessAlert = false;
    private bool _showDeleteModal = false;
    private string _usernameError;
    private string _emailError;

    protected override async Task OnInitializedAsync()
    {
        _users = await UserService.GetAllUsersAsync();
    }

    public void ShowDeleteModal(User user)
    {
        _editingUser = user;
        _showDeleteModal = true;
        StateHasChanged();
    }

    public async Task DeleteUser(User user)
    {
        await UserService.DeleteUserAsync(user);
        _users.Remove(user);
        CloseDeleteModal();
        StateHasChanged();
    }

    public async Task ValidateAndEditUser()
    {
        _usernameError = null;
        _emailError = null;
        if (string.IsNullOrWhiteSpace(_editingUser.Username))
        {
            _usernameError = "Prezývka nemôže byť prázdna";
            StateHasChanged();
            return;
        }

        if (_editingUser.Username != _users.FirstOrDefault(u => u.Id == _editingUser.Id)?.Username)
        {
            if (await IsUsernameTakenAsync(_editingUser.Username))
            {
                _usernameError = "Prezývka je už obsadená";
                StateHasChanged();
                return;
            }
        }

        if (string.IsNullOrWhiteSpace(_editingUser.Email))
        {
            _emailError = "E-mail nemôže byť prázdny";
            StateHasChanged();
            return;
        }

        var emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        if (!Regex.IsMatch(_editingUser.Email, emailPattern))
        {
            _emailError = "E-mail musí byť v správnom formáte (napr. meno@email.com)";
            StateHasChanged();
            return;
        }

        if (_editingUser.Email != _users.FirstOrDefault(u => u.Id == _editingUser.Id)?.Email)
        {
            if (await IsEmailTakenAsync(_editingUser.Email))
            {
                _emailError = "E-mail je už obsadený";
                StateHasChanged();
                return;
            }
        }

        var user = _users.FirstOrDefault(u => u.Id == _editingUser.Id);
        if (user != null)
        {
            user.Username = _editingUser.Username;
            user.Role = _editingUser.Role;
            user.Email = _editingUser.Email;
            await UserService.EditUserAsync(user);
        }

        CloseModal();
        _showSuccessAlert = true;
        await Task.Delay(5000);
        _showSuccessAlert = false;

        StateHasChanged();
    }

    public async Task<bool> IsUsernameTakenAsync(string username)
    {
        var existingUser = await UserService.FindByUsernameAsync(username);
        return existingUser != null;
    }

    public async Task<bool> IsEmailTakenAsync(string email)
    {
        var existingUser = await UserService.FindByEmailAsync(email);
        return existingUser != null;
    }

    public void UpdateShowUsersCount(int count)
    {
        _showUsersCount = count;
        StateHasChanged();
    }

    public void UpdateSkipUsersCount(int count)
    {
        if (_skipUsersCount + count >= 0 && _skipUsersCount + count < _users.Count)
        {
            _skipUsersCount += count;
            StateHasChanged();
        }
    }

    public void SetActualUser(User user)
    {
        _editingUser = new User()
        {
            Id = user.Id,
            Username = user.Username,
            PasswordHash = user.PasswordHash,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };

        _showModal = true;
        StateHasChanged();
    }

    public void CloseModal()
    {
        _usernameError = null;
        _emailError = null;
        _showModal = false;
        _editingUser = new User();
        StateHasChanged();
    }

    public void CloseDeleteModal()
    {
        _showDeleteModal = false;
        StateHasChanged();
    }
}
