using CodeHub.Data.Models;

namespace CodeHub.Components.Pages;

public partial class Home
{
    private List<Problem> _problems = new();
    private int _maxProblemsToShow = 20;
    private int _currentPage = 1;
    private int _totalPages =>  (int)Math.Ceiling((double)_problems.Count / _maxProblemsToShow);

    protected override async Task OnInitializedAsync()
    {
        await LoadProblems();
    }

    public void SetProblemCount(int count)
    {
        _maxProblemsToShow = count;
        _currentPage = 1;
    }

    private void GoToPage(int page)
    {
        if (page < 1 || page > _maxProblemsToShow) return;
        _currentPage = page;
    }

    public async Task LoadProblems()
    {
        _problems = await ProblemService.GetProblemsAsync();
    }
}