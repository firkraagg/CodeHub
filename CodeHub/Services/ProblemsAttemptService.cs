using CodeHub.Data;
using CodeHub.Data.Entities;
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

        public async Task<ProblemAttempt?> GetSolvedProblemByProblemIdAsync(int problemId)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                return await context.ProblemAttempts
                    .Where(sp => sp.problemId == problemId)
                    .FirstOrDefaultAsync();
            }
        }

        public async Task<List<int>> GetProblemIdsByUserIdAsync(int userId)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                return await context.ProblemAttempts
                    .Where(sp => sp.userId == userId && sp.IsSuccessful)
                    .Select(sp => sp.problemId)
                    .Distinct()
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
                    .Distinct()
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

        public async Task<List<ProblemAttempt>> GetProblemsByUserIdAndProblemIdAsync(int userId, int problemId)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                return await context.ProblemAttempts
                    .Where(pa => pa.userId == userId && pa.problemId == problemId)
                    .OrderByDescending(pa => pa.AttemptedAt)
                    .ToListAsync();
            }
        }

        public async Task DeleteAllAttemptsForProblemAsync(int problemId)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                var problemAttempts = await context.ProblemAttempts
                    .Where(pa => pa.problemId == problemId)
                    .ToListAsync();

                if (problemAttempts.Any())
                {
                    context.ProblemAttempts.RemoveRange(problemAttempts);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}