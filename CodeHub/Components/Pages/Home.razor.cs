using CodeHub.Data.Entities;
using CodeHub.Data.Models;
using CodeHub.Services;

namespace CodeHub.Components.Pages;

public partial class Home
{
    private User? _user;
    private List<Problem> _problems = new();
    private List<Problem> _filteredProblems = new();
    private List<Tag> _tags = new();
    private List<Tag> _selectedTags = new();
    private List<int> _completedProblemIds = new();
    private int _selectedDifficulty = -1;
    private string _selectedSort = "";
    private int _selectedTagId = 0;
    private int _maxProblemsToShow = 20;
    private int _currentPage = 1;
    private int _totalPages => (int)Math.Ceiling((double)(_isFiltered ? _filteredProblems.Count : _problems.Count) / _maxProblemsToShow);
    private bool _isFiltered => _selectedDifficulty > -1;


    protected override async Task OnInitializedAsync()
    {
        var userId = ((CustomAuthStateProvider)AuthenticationStateProvider).GetLoggedInUserId();
        if (!string.IsNullOrEmpty(userId))
        {
            _completedProblemIds = await SolvedProblemsService.GetSolvedProblemIdsByUserIdAsync(int.Parse(userId));
        }
        await LoadProblems();
        await LoadTags();
        _selectedTags = new List<Tag>();

        foreach (var problem in _problems)
        {
            problem.Acceptance = await ProblemService.CalculateAcceptanceRateAsync(problem.Id);
        }
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
    }

    public async Task LoadTags()
    {
        _tags = await TagService.GetTagsAsync();
    }

    private void ApplyFilter(int difficulty)
    {
        _selectedDifficulty = difficulty;
        FilterProblems();
    }

    private void ApplySorting(string sort)
    {
        if (_selectedDifficulty < 0)
        {
            _selectedDifficulty = 0;
        }
        _selectedSort = sort;
        FilterProblems();
    }

    private void ToggleTagSelection(Tag tag)
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
            ApplyFilter(0);
        }
        
        FilterProblems();
    }

    private async Task FilterProblems()
    {
        _filteredProblems = _problems;

        if (_selectedTags.Any())
        {
            var selectedTagNames = _selectedTags.Select(tag => tag.Name).ToList();
            _filteredProblems = await TagService.GetProblemsByTagsAsync(selectedTagNames);
        }

        if (_selectedDifficulty > 0)
        {
            _filteredProblems = _filteredProblems.Where(p => p.Difficulty == _selectedDifficulty).ToList();
        }

        _filteredProblems = _selectedSort switch
        {
            "newest" => _filteredProblems.OrderByDescending(p => p.CreatedAt).ToList(),
            "oldest" => _filteredProblems.OrderBy(p => p.CreatedAt).ToList(),
            "easiest" => _filteredProblems.OrderByDescending(p => p.Acceptance).ToList(),
            "hardest" => _filteredProblems.OrderBy(p => p.Acceptance).ToList(),
            _ => _filteredProblems
        };

        _currentPage = 1;
        StateHasChanged();
    }
}