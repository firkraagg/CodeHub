using CodeHub.Data.Entities;
using CodeHub.Data;
using Microsoft.EntityFrameworkCore;

namespace CodeHub.Services
{
    public class ProblemExampleService
    {
        private readonly IDbContextFactory<DatabaseContext> _dbContextFactory;

        public ProblemExampleService(IDbContextFactory<DatabaseContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<ProblemExample?> GetExampleByIdAsync(string id)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                return await context.ProblemExample.FirstOrDefaultAsync(e => e.Id.ToString() == id);
            }
        }

        public async Task UpdateExampleAsync(ProblemExample example)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                context.ProblemExample.Update(example);
                await context.SaveChangesAsync();
            }
        }

        public async Task<List<ProblemExample>> GetExamplesForProblemAsync(int problemId)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                return await context.ProblemExample
                    .Where(e => e.ProblemId == problemId)
                    .ToListAsync();
            }
        }
    }
}
