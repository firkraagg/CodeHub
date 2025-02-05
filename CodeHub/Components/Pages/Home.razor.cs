using CodeHub.Data.Models;

namespace CodeHub.Components.Pages;

public partial class Home
{
    private List<Problem> problems = new();

    protected override async Task OnInitializedAsync()
    {
        problems = await ProblemService.GetProblemsAsync();
    }
}