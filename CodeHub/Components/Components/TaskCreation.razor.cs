using CodeHub.Data.Models;
using CodeHub.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace CodeHub.Components.Components;

public partial class TaskCreation
{
    private Problem _problem = new();
    [SupplyParameterFromForm] private Problem problem { get; set; } = new();
    public async Task HandleCreateProblemFormSubmitAsync(EditContext editContext)
    {
        
    }

    public void CancelCreation()
    {
        NavigationManager.NavigateTo("#");
    }
}