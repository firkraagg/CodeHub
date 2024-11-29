using CodeHub.Data.Entities;

namespace CodeHub.Components.Pages;

public partial class ManageAccounts
{
    private List<User> _users = new List<User>();
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
}