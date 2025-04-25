using BlazorStrap.Utilities;
using CodeHub.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CodeHub.Components.Layout;

public partial class NavMenu
{
    private bool collapseNavMenu = true;
    private bool isLightMode = false;
    private bool _initialized = false;
    private string? NavMenuCssClass => collapseNavMenu ? "collapse" : null;
    private string themeText => isLightMode ? "Svetlý režim" : "Tmavý režim";
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && !_initialized)
        {
            var storedTheme = await JS.InvokeAsync<string>("themeHelper.getTheme");
            isLightMode = storedTheme == "light";
            await JS.InvokeVoidAsync("themeHelper.setTheme", storedTheme);

            _initialized = true;
            StateHasChanged();
        }
    }

    private async Task ToggleTheme(ChangeEventArgs e)
    {
        isLightMode = (bool)e.Value;
        var theme = isLightMode ? "light" : "dark";
        await JS.InvokeVoidAsync("themeHelper.setTheme", theme);
    }

    private string GetThemeClass()
    {
        return isLightMode ? "light-theme" : "dark-theme";
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
}
