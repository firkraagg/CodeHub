using CodeHub.Data;
using CodeHub.Data.Entities;
using CodeHub.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeHub.Services
{
    public class ProblemsAttemptService
    {
        private readonly IDbContextFactory<DatabaseContext> _dbContextFactory;

        public ProblemsAttemptService(IDbContextFactory<DatabaseContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<List<ProblemAttempt>> GetSolvedProblemsByUserIdAsync(int userId)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                return await context.ProblemAttempts
                    .Where(sp => sp.userId == userId)
                    .ToListAsync();
            }
        }

        public async Task<List<ProblemAttempt>> GetSolvedProblemsByProblemIdAsync(int problemId)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                return await context.ProblemAttempts
                    .Where(sp => sp.problemId == problemId)
                    .ToListAsync();
            }
        }

        public async Task<List<int>> GetSolvedProblemIdsByUserIdAsync(int userId)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                return await context.ProblemAttempts
                    .Where(sp => sp.userId == userId && sp.IsSuccessful == true)
                    .Select(sp => sp.problemId)
                    .ToListAsync();
            }
        }

        public async Task AddSolvedProblemAsync(ProblemAttempt solvedProblem)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                context.ProblemAttempts.Add(solvedProblem);
                await context.SaveChangesAsync();
            }
        }

        public async Task<List<User>> GetUsersBySolvedProblemIdAsync(int problemId)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                return await context.ProblemAttempts
                    .Where(sp => sp.problemId == problemId)
                    .Select(sp => sp.User)
                    .ToListAsync();
            }
        }

        public async Task<ProblemAttempt?> GetSolvedProblemByUserIdAsync(int userId)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                return await context.ProblemAttempts
                    .FirstOrDefaultAsync(sp => sp.userId == userId);
            }
        }

        public async Task DeleteSolvedProblemAsync(int userId, int problemId)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                var solvedProblem = await context.ProblemAttempts
                    .FirstOrDefaultAsync(sp => sp.userId == userId && sp.problemId == problemId);

                if (solvedProblem != null)
                {
                    context.ProblemAttempts.Remove(solvedProblem);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
