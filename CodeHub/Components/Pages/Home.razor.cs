using CodeHub.Data.Entities;
using CodeHub.Data.Models;
using CodeHub.Services;

namespace CodeHub.Components.Pages;

public partial class Home
{
    private List<Problem> _problems = new();
    private List<Problem> _filteredProblems = new();
    private List<Tag> _tags = new();
    private int _selectedDifficulty = -1;
    private string _selectedSort = "";
    private int _selectedTagId = 0;
    private int _maxProblemsToShow = 20;
    private int _currentPage = 1;
    private int _totalPages => (int)Math.Ceiling((double)(_isFiltered ? _filteredProblems.Count : _problems.Count) / _maxProblemsToShow);
    private bool _isFiltered => _selectedDifficulty > -1;


    protected override async Task OnInitializedAsync()
    {
        await LoadProblems();
        await LoadTags();
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
        _selectedSort = sort;
        _selectedDifficulty = 0;
        FilterProblems();
    }

    private void FilterProblems()
    {
        _filteredProblems = _selectedDifficulty switch
        {
            0 => _problems,
            _ => _problems.Where(p => p.Difficulty == _selectedDifficulty).ToList()
        };

        _filteredProblems = _selectedSort switch
        {
            "newest" => _filteredProblems.OrderByDescending(p => p.CreatedAt).ToList(),
            "oldest" => _filteredProblems.OrderBy(p => p.CreatedAt).ToList(),
            "easiest" => _filteredProblems.OrderBy(p => p.Acceptance).ToList(),
            "hardest" => _filteredProblems.OrderByDescending(p => p.Acceptance).ToList(),
            _ => _filteredProblems
        };

        _currentPage = 1;
        StateHasChanged();
    }


    private async Task FilterByTag(int tagId)
    {
        _selectedTagId = tagId;
        var selectedTag = _tags.FirstOrDefault(t => t.Id == tagId);

        if (selectedTag != null)
        {
            _filteredProblems = await TagService.GetProblemsByTagAsync(selectedTag.Name);
        }
        _currentPage = 1;
    }
}