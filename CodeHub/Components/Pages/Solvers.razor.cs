using CodeHub.Data.Entities;
using CodeHub.Data.Models;
using CodeHub.Services;
using Microsoft.AspNetCore.Components;

namespace CodeHub.Components.Pages
{
    public partial class Solvers
    {
        [Parameter] public int ProblemId { get; set; }
        private List<User> _users = new();
        private List<DateTime?> _solvedDates = new();
        private Dictionary<int, List<ProblemAttempt>> _userAttempts = new();
        private List<bool> _isSolvedSuccessfully = new();
        private Problem _problem;
        private ProblemAttempt? _actualProblemAttempt;
        private string? _sourceCode;
        private bool _showSourceCode;
        private int _testCasesNumber;

        protected override async Task OnInitializedAsync()
        {
            _users = await ProblemsAttemptService.GetUsersBySolvedProblemIdAsync(ProblemId);
            _problem = await ProblemService.GetProblemByIdAsync(ProblemId);
            var testCases = await TestCaseService.GetTestCasesForProblemAsync(_problem.Id);
            _testCasesNumber = testCases.Count;
            foreach (var user in _users)
            {
                var problems = await ProblemsAttemptService.GetProblemsByUserIdAndProblemIdAsync(user.Id, ProblemId);
                _userAttempts[user.Id] = problems
                    .OrderBy(p => p.AttemptedAt)
                    .ToList();
            }
        }

        public async Task ShowSourceCode(ProblemAttempt attempt)
        {
            _sourceCode = attempt?.SourceCode;
            _showSourceCode = true;
            StateHasChanged();
        }

        public void CloseModal()
        {
            _showSourceCode = false;
            _sourceCode = null;
        }
    }
}