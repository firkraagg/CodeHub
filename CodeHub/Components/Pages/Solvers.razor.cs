using CodeHub.Data.Entities;
using CodeHub.Services;
using Microsoft.AspNetCore.Components;

namespace CodeHub.Components.Pages
{
    public partial class Solvers
    {
        [Parameter] public int ProblemId { get; set; }
        private List<User> _users = new();
        private List<DateTime?> _solvedDates = new();
        private List<bool> _isSolvedSuccessfully = new();
        private ProblemAttempt? _actualProblem;
        private string? _sourceCode;
        private bool _showSourceCode;

        protected override async Task OnInitializedAsync()
        {
            _users = await ProblemsAttemptService.GetUsersBySolvedProblemIdAsync(ProblemId);
            foreach (var user in _users)
            {
                var problem = await ProblemsAttemptService.GetSolvedProblemByUserIdAsync(user.Id);
                _solvedDates.Add(problem?.AttemptedAt);
                _isSolvedSuccessfully.Add(problem?.IsSuccessful ?? false);
            }
        }

        public async Task ShowSourceCode(int userId)
        {
            var problem = await ProblemsAttemptService.GetSolvedProblemByUserIdAsync(userId);
            _sourceCode = problem?.SourceCode;
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