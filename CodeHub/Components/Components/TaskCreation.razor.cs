using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using CodeHub.Data.Entities;
using CodeHub.Data.Models;
using CodeHub.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;

namespace CodeHub.Components.Components;

public partial class TaskCreation
{
    private Problem _problem = new Problem
    {
        Title = string.Empty,
        LanguageID = 0,
        Difficulty = 0,
        Description = string.Empty,
        RequiredInput = string.Empty,
        RequiredOutput = string.Empty,
        Constraints = new List<ProblemConstraint>(),
        Hints = new List<ProblemHint>(),
        Tags = new List<Tag>()
    };
    private List<ProgrammingLanguage> _languages = new();
    private List<Tag> _tags = new();
    private List<string> _selectedTags = new();
    private string _selectedTag = "";
    private ProblemHint _hint = new();
    private List<ProblemHint> _hints = new();
    private ProblemHint _editingHint;
    private ProblemConstraint _constraint = new();
    private List<ProblemConstraint> _constraints = new();
    private ProblemConstraint _editingConstraint;
    private User? _user;
    private bool _showAlert;
    private string _alertColor = "";
    private string _alertMessage = "";
    private bool _showModalHint;
    private bool _showModalConstraint;

    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var userId = ((CustomAuthStateProvider)AuthenticationStateProvider).GetLoggedInUserId();
        _languages = await ProgrammingLanguageService.GetProgrammingLanguagesAsync();
        _tags = await TagService.GetTagsAsync();
        if (!string.IsNullOrEmpty(userId))
        {
            _user = await UserService.GetUserByIdAsync(userId);
        }
    }

    private void AddTag()
    {
        if (!string.IsNullOrEmpty(_selectedTag) && !_selectedTags.Contains(_selectedTag))
        {
            _selectedTags.Add(_selectedTag);
            _selectedTag = "";
        }
    }
    private void RemoveTag()
    {
        if (_selectedTags.Any())
        {
            _selectedTags.RemoveAt(_selectedTags.Count - 1);
        }
    }

    private void RemoveTag(string tag)
    {
        if (_selectedTags.Contains(tag))
        {
            _selectedTags.Remove(tag);
        }
    }

    public async Task HandleCreateProblemFormSubmitAsync()
    {
        var existingProblem = await ProblemService.GetProblemByName(_problem.Title);

        if (existingProblem != null)
        {
            _showAlert = true;
            _alertColor = "alert-danger";
            _alertMessage = "Tento názov úlohy už existuje";
            StateHasChanged();
            return;
        }

        if (_user != null)
        {
            var existingTags = await TagService.GetTagsAsync();
            foreach (var selectedTag in _selectedTags)
            {
                var tag = existingTags.FirstOrDefault(t => t.Name == selectedTag);
                if (tag != null)
                {
                    _problem.Tags.Add(tag);
                }
            }

            foreach (var problemHint in _hints)
            {
                _problem.Hints.Add(problemHint);
            }

            foreach (var problemConstraint in _constraints)
            {
                _problem.Constraints.Add(problemConstraint);
            }

            _problem.UserID = _user.Id;
            await ProblemService.CreateProblemAsync(_problem);
            StateHasChanged();
            NavigationManager.NavigateTo("#");
        }
    }

    public void CancelCreation()
    {
        NavigationManager.NavigateTo("#");
    }

    private void AddHint()
    {
        _hints.Add(new ProblemHint
        {
            Id = _hints.Count + 1,
            Hint = _hint.Hint,
            Problem = _problem,
            ProblemId = 1
        });
        _hint.Hint = string.Empty;
        //_hint.ProblemId = _problem.Id;
        //_hint.Problem = _problem;
        //_hints.Add(_hint);
    }

    private void UpdateHint(ChangeEventArgs e)
    {
        _hint.Hint = e.Value?.ToString();
        StateHasChanged();
    }

    private void RemoveHint()
    {
        if (_hints.Any())
        {
            _hints.RemoveAt(_hints.Count - 1);
        }
    }

    private void RemoveHint(ProblemHint hint)
    {
        if (_hints.Contains(hint))
        {
            _hints.Remove(hint);
        }
    }

    public void ShowModal(ProblemHint hint)
    {
        _showModalHint = true;
        _editingHint = new ProblemHint
        {
            Id = hint.Id,
            Hint = hint.Hint,
            ProblemId = hint.ProblemId
        };
    }

    public void ShowModal(ProblemConstraint constraint)
    {
        _showModalConstraint = true;
        _editingConstraint = new ProblemConstraint
        {
            Id = constraint.Id,
            Constraint = constraint.Constraint,
            ProblemId = constraint.ProblemId
        };
    }

    public void UpdateHintText()
    {
        var hint = _hints.FirstOrDefault(h => h.Id == _editingHint.Id);
        if (hint != null)
        {
            hint.Hint = _editingHint.Hint;
        }

        CloseModal();
        StateHasChanged();
    }

    private void UpdateConstraint(ChangeEventArgs e)
    {
        _constraint.Constraint = e.Value?.ToString();
        StateHasChanged();
    }

    private void AddConstraint()
    {
        _constraints.Add(new ProblemConstraint
        {
            Id = _constraints.Count + 1,
            Constraint = _constraint.Constraint,
            Problem = _problem,
            ProblemId = 1
        });
        _constraint.Constraint = string.Empty;
    }

    public void UpdateConstraintText()
    {
        var constraint = _constraints.FirstOrDefault(c => c.Id == _editingConstraint.Id);
        if (constraint != null)
        {
            constraint.Constraint = _editingConstraint.Constraint;
        }

        CloseModal();
        StateHasChanged();
    }

    private void RemoveConstraint()
    {
        if (_constraints.Any())
        {
            _constraints.RemoveAt(_constraints.Count - 1);
        }
    }

    private void RemoveConstraint(ProblemConstraint constraint)
    {
        if (_constraints.Contains(constraint))
        {
            _constraints.Remove(constraint);
        }
    }

    public void CloseModal()
    {
        _showModalHint = false;
        _showModalConstraint = false;
    }
}