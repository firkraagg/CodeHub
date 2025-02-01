using CodeHub.Data;
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
            using (var context = _dbContextFactory.CreateDbContext())
            {
                var p = new Problem()
                {
                    Title = problem.Title,
                    Description = problem.Description,
                    Acceptance = 0.0,
                    Difficulty = problem.Difficulty,
                    Language = problem.Language,
                    RequiredInput = problem.RequiredInput,
                    RequiredOutput = problem.RequiredOutput,
                    Constraints = problem.Constraints,
                    Hints = problem.Hints,
                    UserID = problem.UserID
                };

                await AddProblemAsync(p);
                return p;
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
                context.Problems.Update(problem);
                await context.SaveChangesAsync();
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
                return await context.Problems.Where(p => p.UserID == userId).ToListAsync();
            }
        }
    }
}
