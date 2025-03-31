using ClosedXML.Excel;
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

        private async Task ExportToExcel()
        {
            using var workbook = new ClosedXML.Excel.XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Výsledky");

            worksheet.Cell(1, 1).Value = "Meno a priezvisko";
            worksheet.Cell(1, 2).Value = "Prezývka";
            worksheet.Cell(1, 3).Value = "E-mail";
            worksheet.Cell(1, 4).Value = "Študijná skupina";
            worksheet.Cell(1, 5).Value = "Poèet bodov";

            worksheet.Cell(1, 1).Style.Fill.BackgroundColor = XLColor.Yellow;
            worksheet.Cell(1, 2).Style.Fill.BackgroundColor = XLColor.Yellow;
            worksheet.Cell(1, 3).Style.Fill.BackgroundColor = XLColor.Yellow;
            worksheet.Cell(1, 4).Style.Fill.BackgroundColor = XLColor.Yellow;
            worksheet.Cell(1, 5).Style.Fill.BackgroundColor = XLColor.Yellow;

            int row = 2;
            int index = 1;

            foreach (var user in _users)
            {
                var bestAttempt = _userAttempts[user.Id]
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
                $"vysledky_{_problem.Title}.xlsx",
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            );
        }
    }
}