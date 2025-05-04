using CodeHub.Data;
using CodeHub.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CodeHub.Services
{
    public class ProblemHintService
    {
        private readonly IDbContextFactory<DatabaseContext> _dbContextFactory;

        public ProblemHintService(IDbContextFactory<DatabaseContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<ProblemHint?> GetHintByIdAsync(string id)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                return await context.ProblemHints.FirstOrDefaultAsync(h => h.Id.ToString() == id);
            }
        }

        public async Task UpdateHintAsync(ProblemHint hint)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                context.ProblemHints.Update(hint);
                await context.SaveChangesAsync();
            }
        }

        public async Task<List<ProblemHint>> GetHintsForProblemAsync(int problemId)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                return await context.ProblemHints
                    .Where(h => h.ProblemId == problemId)
                    .ToListAsync();
            }
        }
    }
}