using CodeHub.Data.Entities;
using CodeHub.Data.Models;
using CodeHub.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Reflection.Metadata;

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
    private List<TestCase> _testCases = new();
    private TestCase _editingTestCase;
    private bool _showModalTestCase;
    private User? _user;
    private bool _showAlert;
    private string _alertColor = "";
    private string _alertMessage = "";
    private bool _showModalHint;
    private bool _showModalConstraint;
    private bool _showModalExample;
    private bool _isEditing;

    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;
    [Parameter] public Problem? ProblemToEdit { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var userId = ((CustomAuthStateProvider)AuthenticationStateProvider).GetLoggedInUserId();
        if (!string.IsNullOrEmpty(userId))
        {
            _user = await UserService.GetUserByIdAsync(userId);
        }

        _languages = await ProgrammingLanguageService.GetProgrammingLanguagesAsync();
        _tags = await TagService.GetTagsAsync();
        _problem.TestCases = [];

        if (ProblemToEdit != null)
        {
            _problem = ProblemToEdit;
            _selectedTags = await TagService.GetTagNamesForProblemAsync(_problem.Id);
            _examples = await ProblemExampleService.GetExamplesForProblemAsync(_problem.Id);
            _constraints = await ProblemConstraintService.GetConstraintsForProblemAsync(_problem.Id);
            _hints = await ProblemHintService.GetHintsForProblemAsync(_problem.Id);
            _testCases = await TestCaseService.GetTestCasesForProblemAsync(_problem.Id);
            _problem.Tags = await TagService.GetTagsForProblemAsync(_problem.Id);
            _problem.Hints = await ProblemHintService.GetHintsForProblemAsync(_problem.Id);
            _problem.Constraints = await ProblemConstraintService.GetConstraintsForProblemAsync(_problem.Id);
            _problem.Examples = await ProblemExampleService.GetExamplesForProblemAsync(_problem.Id);
            _problem.TestCases = await TestCaseService.GetTestCasesForProblemAsync(_problem.Id);
            _isEditing = true;
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

    private async Task HandleFormSubmitAsync()
    {
        if (_problem.Id > 0)
        {
            await HandleEditProblemFormSubmitAsync();
        }
        else
        {
            await HandleCreateProblemFormSubmitAsync();
        }
    }


    public async Task HandleEditProblemFormSubmitAsync()
    {
        var removedHints = _problem.Hints
            .Where(h => !_hints.Any(nh => nh.Hint == h.Hint))
            .ToList();

        foreach (var removed in removedHints)
        {
            _problem.Hints.Remove(removed);
        }

        var uniqueHints = new HashSet<string>(_problem.Hints.Select(h => h.Hint));
        foreach (var problemHint in _hints)
        {
            if (!uniqueHints.Contains(problemHint.Hint))
            {
                _problem.Hints.Add(problemHint);
                uniqueHints.Add(problemHint.Hint);
            }
        }

        var removedConstraints = _problem.Constraints
            .Where(c => !_constraints.Any(nc => nc.Constraint == c.Constraint))
            .ToList();

        foreach (var removed in removedConstraints)
        {
            _problem.Constraints.Remove(removed);
        }

        var uniqueConstraints = new HashSet<string>(_problem.Constraints.Select(c => c.Constraint));
        foreach (var problemConstraint in _constraints)
        {
            if (!uniqueConstraints.Contains(problemConstraint.Constraint))
            {
                _problem.Constraints.Add(problemConstraint);
                uniqueConstraints.Add(problemConstraint.Constraint);
            }
        }

        var removedExamples = _problem.Examples
            .Where(e => !_examples.Any(ne => ne.Input == e.Input && ne.Output == e.Output && ne.Explanation == e.Explanation))
            .ToList();

        foreach (var removed in removedExamples)
        {
            _problem.Examples.Remove(removed);
        }

        var uniqueExamples = new HashSet<string>(_problem.Examples
            .Select(e => $"{e.Input}|{e.Output}|{e.Explanation}"));

        foreach (var problemExample in _examples)
        {
            var exampleKey = $"{problemExample.Input}|{problemExample.Output}|{problemExample.Explanation}";

            if (!uniqueExamples.Contains(exampleKey))
            {
                _problem.Examples.Add(problemExample);
                uniqueExamples.Add(exampleKey);
            }
        }

        var removedTags = _problem.Tags
            .Where(t => !_selectedTags.Contains(t.Name, StringComparer.OrdinalIgnoreCase))
            .ToList();
        foreach (var removed in removedTags) _problem.Tags.Remove(removed);

        var existingTagNames = new HashSet<string>(_problem.Tags.Select(t => t.Name), StringComparer.OrdinalIgnoreCase);
        foreach (var tagName in _selectedTags)
        {
            if (!existingTagNames.Contains(tagName))
            {
                _problem.Tags.Add(new Tag { Name = tagName });
                existingTagNames.Add(tagName);
            }
        }

        var removedTestCases = _problem.TestCases
            .Where(tc => !_testCases.Any(ntc => ntc.Arguments == tc.Arguments && ntc.ExpectedOutput == tc.ExpectedOutput && ntc.OutputType == tc.OutputType))
            .ToList();

        foreach (var removed in removedTestCases)
        {
            _problem.TestCases.Remove(removed);
        }

        var uniqueTestCases = new HashSet<string>(_problem.TestCases.Select(tc => $"{tc.Arguments}-{tc.ExpectedOutput}-{tc.OutputType}"));
        foreach (var newTestCase in _testCases)
        {
            var testCaseKey = $"{newTestCase.Arguments}-{newTestCase.ExpectedOutput}-{newTestCase.OutputType}";
            if (!uniqueTestCases.Contains(testCaseKey))
            {
                _problem.TestCases.Add(newTestCase);
                uniqueTestCases.Add(testCaseKey);
            }
        }

        _problem.UserID = _user.Id;
        await ProblemService.EditProblemAsync(_problem);
        StateHasChanged();
        NavigationManager.NavigateTo("user-profile/problems");
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

            foreach (var problemTestCase in _testCases)
            {
                _problem.TestCases.Add(problemTestCase);
            }

            _problem.UserID = _user.Id;
            await ProblemService.CreateProblemAsync(_problem);
            StateHasChanged();
            NavigationManager.NavigateTo("#");
        }
    }

    public void CancelCreation()
    {
        NavigationManager.NavigateTo("user-profile/problems");
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

    private void ShowModal(TestCase testCase)
    {
        _editingTestCase = new TestCase
        {
            Id = testCase.Id,
            Arguments = testCase.Arguments,
            ExpectedOutput = testCase.ExpectedOutput,
            OutputType = testCase.OutputType
        };
        _showModalTestCase = true;
    }

    private void ShowModalTestCase()
    {
        _editingTestCase = new TestCase { Id = 0 };
        _showModalTestCase = true;
    }

    private void CreateTestCase()
    {
        _editingTestCase.Id = _testCases.Count + 1;
        _testCases.Add(_editingTestCase);
        CloseModal();
    }

    private void UpdateTestCase()
    {
        var index = _testCases.FindIndex(tc => tc.Id == _editingTestCase.Id);
        if (index >= 0)
        {
            _testCases[index] = _editingTestCase;
        }
        CloseModal();
    }

    private void HandleValidSubmit()
    {
        if (_editingTestCase.Id == 0)
            CreateTestCase();
        else
            UpdateTestCase();

        CloseModal();
    }

    private void RemoveTestCase()
    {
        if (_testCases.Any())
        {
            _testCases.RemoveAt(_testCases.Count - 1);
        }
    }

    private void RemoveTestCase(TestCase testCase)
    {
        if (_testCases.Contains(testCase))
        {
            _testCases.Remove(testCase);
        }
    }

    public void CloseModal()
    {
        _showModalHint = false;
        _showModalConstraint = false;
        _showModalExample = false;
        _showModalTestCase = false;
    }
}