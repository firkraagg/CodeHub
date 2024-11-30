using CodeHub.Data.Entities;

namespace CodeHub.Components.Pages;

public partial class ManageAccounts
{
    private List<User> _users = new List<User>();
    private int _showUsersCount = 20;
    private int _skipUsersCount = 0;
    private User _actualUser = new User();
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

    public async Task EditUser(User user)
    {
        
        //await UserService.EditUserAsync(user);
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
        _actualUser = user;
        StateHasChanged();
    }
}