using CodeHub.Data;
using CodeHub.Data.Entities;
using CodeHub.Data.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CodeHub.Services
{
    public class ProblemService
    {
        private readonly IDbContextFactory<DatabaseContext> _dbContextFactory;
        [Inject] TagService TagService { get; set; } = null!;
        [Inject] private ProblemConstraintService ProblemConstraintService { get; set; } = null!;

        public ProblemService(IDbContextFactory<DatabaseContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<Problem?> CreateProblemAsync(Problem problem)
        {
            try
            {
                using (var context = _dbContextFactory.CreateDbContext())
                {
                    var user = await context.Users.Include(u => u.Problems)
                        .FirstOrDefaultAsync(u => u.Id == problem.UserID);

                    if (user == null)
                    {
                        return null;
                    }

                    var newProblem = new Problem()
                    {
                        Title = problem.Title,
                        Description = problem.Description,
                        Acceptance = 0.0,
                        Difficulty = problem.Difficulty,
                        Points = problem.Points,
                        LanguageID = problem.LanguageID,
                        DefaultCode = problem.DefaultCode,
                        CreatedAt = DateTime.Now,
                        ReleaseDate = problem.ReleaseDate,
                        Week = problem.Week,
                        Examples = new List<ProblemExample>(),
                        Constraints = new List<ProblemConstraint>(),
                        Hints = new List<ProblemHint>(),
                        Tags = problem.Tags?.Select(t => new Tag { Name = t.Name }).ToList(),
                        TestCases = new List<TestCase>(),
                        UserID = problem.UserID
                    };

                    context.Problems.Add(newProblem);
                    await context.SaveChangesAsync();

                    if (problem.Hints.Any())
                    {
                        foreach (var hint in problem.Hints)
                        {
                            var newHint = new ProblemHint
                            {
                                ProblemId = newProblem.Id,
                                Hint = hint.Hint
                            };
                            context.ProblemHints.Add(newHint);
                        }
                        await context.SaveChangesAsync();
                    }

                    if (problem.Constraints.Any())
                    {
                        foreach (var constraint in problem.Constraints)
                        {
                            var newConstraint = new ProblemConstraint
                            {
                                ProblemId = newProblem.Id,
                                Constraint = constraint.Constraint
                            };
                            context.ProblemConstraints.Add(newConstraint);
                        }
                        await context.SaveChangesAsync();
                    }

                    if (problem.Examples.Any())
                    {
                        foreach (var example in problem.Examples)
                        {
                            var newExample = new ProblemExample
                            {
                                ProblemId = newProblem.Id,
                                Input = example.Input,
                                Output = example.Output,
                                Explanation = example.Explanation ?? ""
                            };
                            context.ProblemExamples.Add(newExample);
                        }
                        await context.SaveChangesAsync();
                    }

                    if (problem.TestCases.Any())
                    {
                        foreach (var testCase in problem.TestCases)
                        {
                            var newTestCase = new TestCase
                            {
                                ProblemId = newProblem.Id,
                                Arguments = testCase.Arguments,
                                ExpectedOutput = testCase.ExpectedOutput,
                                OutputType = testCase.OutputType
                            };
                            context.TestCases.Add(newTestCase);
                        }
                        await context.SaveChangesAsync();
                    }

                    return newProblem;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task AddProblemAsync(Problem problem)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                await context.Problems.AddAsync(problem);
                await context.SaveChangesAsync();
            }
        }

        public async Task EditProblemAsync(Problem problem)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                var existingProblem = await context.Problems
                    .Include(p => p.Constraints)
                    .Include(p => p.Hints)
                    .Include(p => p.Examples)
                    .Include(p => p.Tags)
                    .Include(p => p.TestCases)
                    .FirstOrDefaultAsync(p => p.Id == problem.Id);

                if (existingProblem != null)
                {
                    context.Attach(existingProblem);
                    context.Entry(existingProblem).State = EntityState.Modified;

                    existingProblem.Title = problem.Title;
                    existingProblem.Difficulty = problem.Difficulty;
                    existingProblem.Points = problem.Points;
                    existingProblem.ProgrammingLanguage = problem.ProgrammingLanguage;
                    existingProblem.LanguageID = problem.LanguageID;
                    existingProblem.Description = problem.Description;
                    existingProblem.DefaultCode = problem.DefaultCode;
                    existingProblem.Week = problem.Week;
                    existingProblem.ReleaseDate = problem.ReleaseDate;

                    await context.SaveChangesAsync();

                    var existingConstraints = existingProblem.Constraints.ToList();

                    foreach (var constraint in existingConstraints)
                    {
                        if (!problem.Constraints.Any(c => c.Constraint == constraint.Constraint))
                        {
                            context.ProblemConstraints.Remove(constraint);
                        }
                    }

                    foreach (var constraint in problem.Constraints)
                    {
                        if (!existingConstraints.Any(c => c.Constraint == constraint.Constraint))
                        {
                            context.ProblemConstraints.Add(new ProblemConstraint
                            {
                                Constraint = constraint.Constraint,
                                ProblemId = existingProblem.Id
                            });
                        }
                    }

                    await context.SaveChangesAsync();

                    var existingHints = existingProblem.Hints.ToList();

                    foreach (var hint in existingHints)
                    {
                        if (!problem.Hints.Any(h => h.Hint == hint.Hint))
                        {
                            context.ProblemHints.Remove(hint);
                        }
                    }

                    foreach (var hint in problem.Hints)
                    {
                        if (!existingHints.Any(h => h.Hint == hint.Hint))
                        {
                            context.ProblemHints.Add(new ProblemHint
                            {
                                Hint = hint.Hint,
                                ProblemId = existingProblem.Id
                            });
                        }
                    }

                    await context.SaveChangesAsync();

                    var existingExamples = existingProblem.Examples.ToList();

                    foreach (var example in existingExamples)
                    {
                        if (!problem.Examples.Any(e => e.Input == example.Input && e.Output == example.Output && e.Explanation == example.Explanation))
                        {
                            context.ProblemExamples.Remove(example);
                        }
                    }

                    foreach (var example in problem.Examples)
                    {
                        if (!existingExamples.Any(e => e.Input == example.Input && e.Output == example.Output && e.Explanation == example.Explanation))
                        {
                            context.ProblemExamples.Add(new ProblemExample
                            {
                                Input = example.Input,
                                Output = example.Output,
                                Explanation = example.Explanation,
                                ProblemId = existingProblem.Id
                            });
                        }
                    }

                    await context.SaveChangesAsync();

                    var existingTagNames = existingProblem.Tags.Select(t => t.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
                    var selectedTagNames = problem.Tags.Select(t => t.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);

                    var tagsToRemove = existingProblem.Tags
                        .Where(t => !selectedTagNames.Contains(t.Name))
                        .ToList();
                    foreach (var tag in tagsToRemove)
                    {
                        existingProblem.Tags.Remove(tag);
                    }

                    foreach (var tagName in selectedTagNames.Except(existingTagNames))
                    {
                        var tag = await context.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
                        if (tag == null)
                        {
                            tag = new Tag { Name = tagName };
                            context.Tags.Add(tag);
                            await context.SaveChangesAsync();
                        }
                        existingProblem.Tags.Add(tag);
                    }

                    await context.SaveChangesAsync();

                    var existingTestCases = existingProblem.TestCases.ToList();
                    foreach (var testCase in existingTestCases)
                    {
                        if (!problem.TestCases.Any(tc =>
                            tc.Arguments == testCase.Arguments &&
                            tc.ExpectedOutput == testCase.ExpectedOutput &&
                            tc.OutputType == testCase.OutputType))
                        {
                            context.Remove(testCase);
                        }
                    }

                    foreach (var testCase in problem.TestCases)
                    {
                        if (!existingTestCases.Any(tc =>
                            tc.Arguments == testCase.Arguments &&
                            tc.ExpectedOutput == testCase.ExpectedOutput &&
                            tc.OutputType == testCase.OutputType))
                        {
                            context.Add(new TestCase
                            {
                                Arguments = testCase.Arguments,
                                ExpectedOutput = testCase.ExpectedOutput,
                                OutputType = testCase.OutputType,
                                ProblemId = existingProblem.Id
                            });
                        }
                    }

                    await context.SaveChangesAsync();
                }
            }
        }

        public async Task DeleteProblemAsync(Problem problem)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                context.Problems.Remove(problem);
                await context.SaveChangesAsync();
            }
        }

        public async Task<List<Problem>> GetProblemsAsync()
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                var problems = await context.Problems.ToListAsync();
                return problems.Where(p => p.IsVisible).ToList();
            }
        }

        public async Task<Problem> GetProblemByIdAsync(int id)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                return await context.Problems.FirstOrDefaultAsync(p => p.Id == id);
            }
        }

        public async Task<List<Problem>> GetProblemsByUserIdAsync(int userId)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                var problems = await context.Problems
                    .Where(p => p.UserID == userId)
                    .Include(p => p.Tags)
                    .ToListAsync();

                return problems;
            }
        }

        public async Task<Problem?> GetProblemByName(string title)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                return await context.Problems.FirstOrDefaultAsync(p => p.Title == title);
            }
        }

        public async Task<double> CalculateAcceptanceRateAsync(int problemId)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                var totalAttempts = await context.ProblemAttempts
                .Where(a => a.problemId == problemId)
                .CountAsync();

                if (totalAttempts == 0) return 0;

                var successfulAttempts = await context.ProblemAttempts
                    .Where(a => a.problemId == problemId && a.IsSuccessful)
                    .CountAsync();

                return (double)successfulAttempts / totalAttempts;
            }
        }
    }
}