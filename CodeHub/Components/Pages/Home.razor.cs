using CodeHub.Data.Models;

namespace CodeHub.Components.Pages;

public partial class Home
{
    private List<Problem> _problems = new();
    private int _maxProblemsToShow = 20;

    protected override async Task OnInitializedAsync()
    {
        _problems = await ProblemService.GetProblemsAsync();
    }

    public void SetProblemCount(int count)
    {
        _maxProblemsToShow = count;
    }
}