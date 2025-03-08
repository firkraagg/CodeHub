using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using CodeHub.Data.Entities;
using CodeHub.Data.Models;
using CodeHub.Services;
using Markdig;
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
        Examples = new List<ProblemExample>(),
        Constraints = new List<ProblemConstraint>(),
        Hints = new List<ProblemHint>(),
        Tags = new List<Tag>()
    };
    private List<ProgrammingLanguage> _languages = new();
    private List<Tag> _tags = new();
    private List<string> _selectedTags = new();
    private string _selectedTag = "";
    private string _customTag = "";
    private ProblemHint _hint = new();
    private List<ProblemHint> _hints = new();
    private ProblemHint _editingHint;
    private ProblemConstraint _constraint = new();
    private List<ProblemConstraint> _constraints = new();
    private ProblemConstraint _editingConstraint;
    private ProblemExample _example = new();
    private List<ProblemExample> _examples = new();
    private ProblemExample _editingExample;
    private User? _user;
    private bool _showAlert;
    private string _alertColor = "";
    private string _alertMessage = "";
    private bool _showModalHint;
    private bool _showModalConstraint;
    private bool _showModalExample;

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
        } 
        else if (!string.IsNullOrEmpty(_customTag) && !_selectedTags.Contains(_customTag))
        {
            _selectedTags.Add(_customTag);
        }

        _selectedTag = "";
        _customTag = "";
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

    private async Task OnCodeInput(ChangeEventArgs e)
    {
        _problem.DefaultCode = e.Value.ToString();
    }

    private void OnCustomTagInput(ChangeEventArgs e)
    {
        _customTag = e.Value.ToString();
        StateHasChanged();
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
            foreach (var selectedTag in _selectedTags)
            {
                var tag = new Tag
                {
                    Name = selectedTag
                };
                
                _problem.Tags.Add(tag);
            }

            foreach (var problemHint in _hints)
            {
                _problem.Hints.Add(problemHint);
            }

            foreach (var problemConstraint in _constraints)
            {
                _problem.Constraints.Add(problemConstraint);
            }

            foreach (var problemExample in _examples)
            {
                _problem.Examples.Add(problemExample);
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

    public void ShowModal(ProblemExample example)
    {
        _showModalExample = true;
        _editingExample = new ProblemExample()
        {
            Id = example.Id,
            Input = example.Input,
            Output = example.Output,
            Explanation = example.Explanation,
            ProblemId = example.ProblemId,
            Problem = _problem
        };
    }

    public void ShowModalExample()
    {
        _editingExample = new();
        _showModalExample = true;
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

    private void AddExample()
    {
        _examples.Add(new ProblemExample
        {
            Id = _examples.Count + 1,
            Input = _example.Input,
            Output = _example.Output,
            Explanation = _example.Explanation,
            Problem = _problem,
            ProblemId = 1
        });
        _example.Input = string.Empty;
        _example.Output = string.Empty;
        _example.Explanation = string.Empty;
    }

    private void RemoveExample()
    {
        if (_examples.Any())
        {
            _examples.RemoveAt(_examples.Count - 1);
        }
    }

    private void RemoveExample(ProblemExample example)
    {
        if (_examples.Contains(example))
        {
            _examples.Remove(example);
        }
    }

    public void CreateExample()
    {
        if (!string.IsNullOrWhiteSpace(_editingExample.Input) &&
            !string.IsNullOrWhiteSpace(_editingExample.Output))
        {
            var newExample = new ProblemExample
            {
                Id = _examples.Count + 1,
                Input = _editingExample.Input,
                Output = _editingExample.Output,
                Explanation = _editingExample.Explanation,
                ProblemId = _problem.Id,  
                Problem = _problem
            };

            _examples.Add(newExample);
            CloseModal();
        }
    }


    public void UpdateExample()
    {
        var example = _examples.FirstOrDefault(e => e.Id == _editingExample.Id);
        if (example != null)
        {
            example.Input = _editingExample.Input;
            example.Output = _editingExample.Output;
            example.Explanation = _editingExample.Explanation;
        }

        CloseModal();
    }

    public void CloseModal()
    {
        _showModalHint = false;
        _showModalConstraint = false;
        _showModalExample = false;
    }
}