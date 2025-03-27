using CodeHub.Services;
using Microsoft.AspNetCore.Components;

namespace CodeHub.Components.Layout;

public partial class NavMenu
{
    private bool collapseNavMenu = true;
    private bool isLightMode = false;
    private string? NavMenuCssClass => collapseNavMenu ? "collapse" : null;

    private void ToggleTheme(ChangeEventArgs e)
    {
        isLightMode = (bool)e.Value;
    }

    private void ToggleNavMenu()
    {
        collapseNavMenu = !collapseNavMenu;
    }
    private async Task Logout()
    {
        await UserService.LogoutUserAsync();
        NavigationManager.NavigateTo("/", true);
        StateHasChanged();
    }

    private string GetThemeClass()
    {
        return isLightMode ? "light-mode" : "dark-mode";
    }
}
