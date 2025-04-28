using CodeHub.Data.Entities;
using CodeHub.Data.Models;
using CodeHub.Services;
using Microsoft.AspNetCore.Components.Routing;

namespace CodeHub.Components.Pages;

public partial class Home
{
    private User? _user;
    private List<Problem> _problems = new();
    private List<Problem> _filteredProblems = new();
    private List<Tag> _tags = new();
    private List<Tag> _selectedTags = new();
    private List<int> _completedProblemIds = new();
    private List<int> _availableWeeks = new();
    private HashSet<int> _selectedWeeks = new();
    private int? _activeWeek = null;
    private int _selectedDifficulty = -1;
    private string _selectedSort = "";
    private int _selectedTagId = 0;
    private int _maxProblemsToShow = 20;
    private int _currentPage = 1;
    private int _totalPages => (int)Math.Ceiling((double)(_isFiltered ? _filteredProblems.Count : _problems.Count) / _maxProblemsToShow);
    private bool _isFiltered => _selectedDifficulty > -1;
    private bool _problemsAreLoading;
    private bool _isFiltering;

    protected override async Task OnInitializedAsync()
    {
        _problemsAreLoading = true;

        var userId = ((CustomAuthStateProvider)AuthenticationStateProvider).GetLoggedInUserId();
        if (!string.IsNullOrEmpty(userId))
        {
            _completedProblemIds = await SolvedProblemsService.GetProblemIdsByUserIdAsync(int.Parse(userId));
        }

        await LoadProblems();
        ProblemCacheService.SetProblems(_problems);

        await LoadTags();
        UpdateAvailableWeeks();
        _selectedTags = new List<Tag>();

        foreach (var problem in _problems)
        {
            problem.Acceptance = await ProblemService.CalculateAcceptanceRateAsync(problem.Id);
        }

        _problemsAreLoading = false;
    }

    private void UpdateAvailableWeeks()
    {
        _availableWeeks = _problems
            .Select(p => p.Week)
            .Distinct()
            .Where(w => w <= 13)
            .OrderBy(w => w)
            .ToList();
    }


    private async Task ToggleWeekSelection(int week)
    {
        if (_selectedWeeks.Contains(week))
        {
            _selectedWeeks.Remove(week);
        }
        else
        {
            _selectedWeeks.Add(week);
        }

        if (!(_selectedDifficulty > 0))
        {
            await ApplyFilter(0);
        }

        _activeWeek = null;
        await FilterProblems();
    }

    private async Task ToggleActiveWeek(int week)
    {
        if (!(_selectedDifficulty > 0))
        {
            await ApplyFilter(0);
        }

        _selectedWeeks.Clear();
        _activeWeek = week;
        _selectedWeeks.Add(week);
        await FilterProblems();
    }

    public void SetProblemCount(int count)
    {
        _maxProblemsToShow = count;
        _currentPage = 1;
    }

    private void GoToPage(int page)
    {
        if (page < 1 || page > _totalPages) return;
        _currentPage = page;
    }

    public async Task LoadProblems()
    {
        _problems = await ProblemService.GetProblemsAsync();
        _filteredProblems = _problems;
    }

    public async Task LoadTags()
    {
        _tags = await TagService.GetTagsAsync();
    }

    private async Task ApplyFilter(int difficulty)
    {
        _selectedDifficulty = difficulty;
        await FilterProblems();
    }

    public async Task SelectAll()
    {
        _activeWeek = null;
        _selectedDifficulty = 0;
        _selectedWeeks.Clear();
        _selectedTags.Clear();
        await FilterProblems();
    }

    private async Task ApplySorting(string sort)
    {
        if (_selectedDifficulty < 0)
        {
            _selectedDifficulty = 0;
        }

        _selectedSort = sort;
        await FilterProblems();
    }

    private async Task ToggleTagSelection(Tag tag)
    {
        if (_selectedTags.Contains(tag))
        {
            _selectedTags.Remove(tag);
        }
        else
        {
            _selectedTags.Add(tag);
        }

        if (!(_selectedDifficulty > 0))
        {
            await ApplyFilter(0);
        }

        await FilterProblems();
    }

    private async Task FilterProblems()
    {
        _isFiltering = true;
        StateHasChanged();
        List<Problem> filtered = _problems;


        if (_selectedTags.Any())
        {
            var selectedTagNames = _selectedTags.Select(tag => tag.Name).ToList();
            filtered = await TagService.GetProblemsByTagsAsync(selectedTagNames);
        }

        if (_selectedWeeks.Any())
        {
            filtered = filtered.Where(p => _selectedWeeks.Contains(p.Week)).ToList();
        }

        if (_selectedDifficulty > 0)
        {
            filtered = filtered.Where(p => p.Difficulty == _selectedDifficulty).ToList();
        }

        filtered = _selectedSort switch
        {
            "newest" => filtered.OrderByDescending(p => p.CreatedAt).ToList(),
            "oldest" => filtered.OrderBy(p => p.CreatedAt).ToList(),
            "easiest" => filtered.OrderByDescending(p => p.Acceptance).ToList(),
            "hardest" => filtered.OrderBy(p => p.Acceptance).ToList(),
            _ => filtered
        };

        _filteredProblems = filtered;
        _isFiltering = false;
        _currentPage = 1;
        StateHasChanged();
    }
}