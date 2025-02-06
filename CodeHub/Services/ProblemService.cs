using CodeHub.Data;
using CodeHub.Data.Entities;
using CodeHub.Data.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace CodeHub.Services
{
    public class ProblemService
    {
        private readonly IDbContextFactory<DatabaseContext> _dbContextFactory;

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
                        LanguageID = problem.LanguageID,
                        RequiredInput = problem.RequiredInput,
                        RequiredOutput = problem.RequiredOutput,
                        Constraints = problem.Constraints,
                        Hints = problem.Hints,
                        Tags = problem.Tags,
                        UserID = problem.UserID
                    };

                    user.Problems.Add(newProblem);
                    await AddProblemAsync(newProblem);
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
                    .Include(p => p.Tags)
                    .FirstOrDefaultAsync(p => p.Id == problem.Id);

                if (existingProblem != null)
                {
                    context.Entry(existingProblem).State = EntityState.Detached;
                    context.Problems.Attach(problem);
                    context.Entry(problem).State = EntityState.Modified;
                    foreach (var tag in problem.Tags)
                    {
                        context.Tag.Attach(tag);
                        context.Entry(tag).State = EntityState.Unchanged;
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
                return await context.Problems.ToListAsync();
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
    }
}
