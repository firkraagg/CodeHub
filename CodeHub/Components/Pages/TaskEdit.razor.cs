using CodeHub.Data.Models;
using CodeHub.Services;
using Microsoft.AspNetCore.Components;

namespace CodeHub.Components.Pages
{
    public partial class TaskEdit
    {
        [Parameter] public int ProblemId { get; set; }
        private Problem? _problem;
        private bool isLoading = true;

        [Inject] private ProblemService ProblemService { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            _problem = await ProblemService.GetProblemByIdAsync(ProblemId);
            isLoading = false;
            StateHasChanged();
        }
    }
}