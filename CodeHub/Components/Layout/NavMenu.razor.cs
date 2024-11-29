using Microsoft.AspNetCore.Components;

namespace CodeHub.Components.Layout;

public partial class NavMenu
{
    private bool collapseNavMenu = true;

    private string? NavMenuCssClass => collapseNavMenu ? "collapse" : null;

    private void ToggleNavMenu()
    {
        collapseNavMenu = !collapseNavMenu;
    }
    private async Task Logout()
    {
        await UserService.LogoutUserAsync();
    }
}
