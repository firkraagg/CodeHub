using CodeHub.Data.Entities;
using CodeHub.Data.Models;
using CodeHub.Services;

namespace CodeHub.Components.Pages;

public partial class Home
{
    private List<Problem> _problems = new();
    private List<Problem> _filteredProblems = new();
    private List<Tag> _tags = new();
    private int _selectedDifficulty = 0;
    private int _selectedTagId = 0;
    private int _maxProblemsToShow = 20;
    private int _currentPage = 1;
    private int _totalPages => (int)Math.Ceiling((double)(IsFiltered ? _filteredProblems.Count : _problems.Count) / _maxProblemsToShow);
    private bool IsFiltered => _selectedDifficulty > 0;


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
        if (page < 1 || page > _maxProblemsToShow) return;
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

    private void FilterByDifficulty(int difficulty)
    {
        _selectedDifficulty = difficulty;
        _filteredProblems = difficulty == 0
            ? _problems
            : _problems.Where(p => p.Difficulty == difficulty).ToList();
        _currentPage = 1;
    }

    private void FilterByTag(int tagId)
    {
        _selectedTagId = tagId;
        _filteredProblems = tagId == 0
            ? _problems
            : _problems.Where(p => p.Tags != null && p.Tags.Any(t => t.Id == tagId)).ToList();
        _currentPage = 1;
    }
}