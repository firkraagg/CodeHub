using ClosedXML.Excel;
using CodeHub.Data.Entities;
using CodeHub.Data.Models;
using CodeHub.Services;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Components;

namespace CodeHub.Components.Pages
{
    public partial class Solvers
    {
        [Parameter] public int ProblemId { get; set; }
        private List<User> _users = new();
        private Dictionary<int, List<ProblemAttempt>> _allUserAttempts = new();
        private List<ProblemAttempt> _userAttempts = new();
        private List<ProblemAttempt> _filteredAttempts = new();
        private Problem _problem;
        private string? _sourceCode;
        private bool _showSourceCode;
        private int _testCasesNumber;
        private bool _showDeleteModal = false;
        private string _deleteModalText = string.Empty;
        private string _actionName = string.Empty;
        private int _currentPage = 1;
        private int _maxAttemptsToShow = 10;
        private bool _attemptsAreLoading = false;
        private int _totalPages => (int)Math.Ceiling((double)_userAttempts.Count / _maxAttemptsToShow);
        private string _attemptFilter = "all";
        private string _attemptSort = "date";
        private bool _isFiltering = false;

        protected override async Task OnInitializedAsync()
        {
            _users = await ProblemsAttemptService.GetUsersBySolvedProblemIdAsync(ProblemId);
            _problem = await ProblemService.GetProblemByIdAsync(ProblemId);
            var testCases = await TestCaseService.GetTestCasesForProblemAsync(_problem.Id);
            _testCasesNumber = testCases.Count;
            await LoadUserAttempts();
            _filteredAttempts = _userAttempts;
        }

        public async Task ShowSourceCode(ProblemAttempt attempt)
        {
            _sourceCode = attempt?.SourceCode;
            _showSourceCode = true;
            StateHasChanged();
        }

        public async Task LoadUserAttempts()
        {
            foreach (var user in _users)
            {
                var problems = await ProblemsAttemptService.GetProblemsByUserIdAndProblemIdAsync(user.Id, ProblemId);
                _allUserAttempts[user.Id] = problems
                    .OrderBy(p => p.AttemptedAt)
                    .ToList();
                _userAttempts.AddRange(problems);
            }

            _userAttempts = _userAttempts.OrderByDescending(p => p.AttemptedAt).ToList();
        }

        public void CloseModal()
        {
            _showSourceCode = false;
            _sourceCode = null;
        }

        private async Task ExportToExcel()
        {
            using var workbook = new ClosedXML.Excel.XLWorkbook();
            var worksheet = workbook.Worksheets.Add("V˝sledky");

            worksheet.Cell(1, 1).Value = "Meno a priezvisko";
            worksheet.Cell(1, 2).Value = "Prez˝vka";
            worksheet.Cell(1, 3).Value = "E-mail";
            worksheet.Cell(1, 4).Value = "ätudijn· skupina";
            worksheet.Cell(1, 5).Value = "PoËet bodov";

            worksheet.Cell(1, 1).Style.Fill.BackgroundColor = XLColor.Yellow;
            worksheet.Cell(1, 2).Style.Fill.BackgroundColor = XLColor.Yellow;
            worksheet.Cell(1, 3).Style.Fill.BackgroundColor = XLColor.Yellow;
            worksheet.Cell(1, 4).Style.Fill.BackgroundColor = XLColor.Yellow;
            worksheet.Cell(1, 5).Style.Fill.BackgroundColor = XLColor.Yellow;

            int row = 2;
            int index = 1;

            foreach (var user in _users)
            {
                var bestAttempt = _allUserAttempts[user.Id]
                    .OrderByDescending(attempt => attempt.Points)
                    .FirstOrDefault();

                if (bestAttempt != null)
                {
                    worksheet.Cell(row, 1).Value = user.DisplayName;
                    worksheet.Cell(row, 2).Value = user.Username;
                    worksheet.Cell(row, 3).Value = user.Email;
                    worksheet.Cell(row, 4).Value = user.Group;
                    worksheet.Cell(row, 5).Value = bestAttempt.Points;

                    row++;
                    index++;
                }
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            await DownloadFileService.DownloadFile(
                $"v˝sledky_{_problem.Title}.xlsx",
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            );
        }

        public void ShowDeleteAllAttemptsModal()
        {
            _deleteModalText = "Naozaj chcete odstr·niù vöetky pokusy?";
            _actionName = "Odstr·niù";
            _showDeleteModal = true;
            StateHasChanged();
        }

        public void CloseDeleteModal()
        {
            _showDeleteModal = false;
            StateHasChanged();
        }

        public async Task DeleteAllAttempts()
        {
            await ProblemsAttemptService.DeleteAllAttemptsForProblemAsync(ProblemId);
            _showDeleteModal = false;
             _users = await ProblemsAttemptService.GetUsersBySolvedProblemIdAsync(ProblemId);
            await LoadUserAttempts();
            _userAttempts.Clear();
            _filteredAttempts = _userAttempts;
            StateHasChanged();
        }

        private void GoToPage(int page)
        {
            if (page < 1 || page > _totalPages) return;
            _currentPage = page;
        }

        public void SetAttemptsToShowCount(int count)
        {
            _maxAttemptsToShow = count;
            _currentPage = 1;
        }

        private async Task ApplyAttemptFilter(string filter)
        {
            _attemptFilter = filter;
            await FilterAttempts();
        }

        private async Task ApplyAttemptSort(string sort)
        {
            _attemptSort = sort;
            await FilterAttempts();
        }

        private async Task FilterAttempts()
        {
            _isFiltering = true;
            StateHasChanged();

            var filtered = _userAttempts;

            filtered = _attemptFilter switch
            {
                "success" => filtered.Where(a => a.IsSuccessful).ToList(),
                "fail" => filtered.Where(a => !a.IsSuccessful).ToList(),
                _ => filtered
            };

            filtered = _attemptSort switch
            {
                "date" => filtered.OrderByDescending(a => a.AttemptedAt).ToList(),
                "user" => filtered.OrderBy(a => _users.FirstOrDefault(u => u.Id == a.userId)?.DisplayName).ToList(),
                _ => filtered
            };

            _filteredAttempts = filtered;
            _isFiltering = false;
            _currentPage = 1;

            StateHasChanged();
        }
    }
}