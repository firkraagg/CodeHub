using Microsoft.AspNetCore.Components;

namespace CodeLab.Components;

public partial class UserAuthForm
{
    [Parameter] public string Title { get; set; }
    [Parameter] public bool showNickname { get; set; }
    [Parameter] public bool ShowConfirmPassword { get; set; }
    [Parameter] public bool ShowPasswordLayout { get; set; }
    [Parameter] public bool ShowRememberMe { get; set; }
    [Parameter] public string ButtonLabel { get; set; }
}