using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CodeHub.Components.Layout;

public partial class NavMenu
{
    private bool collapseNavMenu = true;
    //private bool _isDarkMode;

    //MudTheme MyCustomTheme = new MudTheme()
    //{
    //    PaletteLight = new PaletteLight()
    //    {
    //        Primary = Colors.DeepPurple.Darken2,
    //        Secondary = Colors.Green.Accent4,
    //        AppbarBackground = Colors.BlueGray.Darken3,
    //    },
    //    PaletteDark = new PaletteDark()
    //    {
    //        Primary = Colors.Blue.Darken4,
    //        Secondary = Colors.Green.Accent4,
    //        AppbarBackground = Colors.Gray.Darken3,
    //    },
    //};

    private string? NavMenuCssClass => collapseNavMenu ? "collapse" : null;

    private void ToggleNavMenu()
    {
        collapseNavMenu = !collapseNavMenu;
    }
}
