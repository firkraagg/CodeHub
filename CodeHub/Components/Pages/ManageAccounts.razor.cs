using CodeHub.Data.Entities;

namespace CodeHub.Components.Pages;

public partial class ManageAccounts
{
    private List<User> _users = new List<User>();
    private int _showUsersCount = 10;
    private int _skipUsersCount = 0;
    private User _editingUser = new User();

    protected override async Task OnInitializedAsync()
    {
        _users = await UserService.GetAllUsersAsync();
    }

    public async Task DeleteUser(User user)
    {
        await UserService.DeleteUserAsync(user);
        _users.Remove(user);
        StateHasChanged();
    }

    public async Task EditUser()
    {
        var user = _users.FirstOrDefault(u => u.Id == _editingUser.Id);
        if (user != null)
        {
            user.Username = _editingUser.Username;
            user.Role = _editingUser.Role;
            user.Email = _editingUser.Email;
            await UserService.EditUserAsync(user);
        }

        StateHasChanged();
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

        StateHasChanged();
    }
}