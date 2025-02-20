using CodeHub.Data.Entities;
using CodeHub.Data;
using Microsoft.EntityFrameworkCore;

namespace CodeHub.Services
{
    public class ProblemConstraintService
    {
        private readonly IDbContextFactory<DatabaseContext> _dbContextFactory;

        public ProblemConstraintService(IDbContextFactory<DatabaseContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<ProblemConstraint?> GetConstraintByIdAsync(string id)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                return await context.ProblemConstraint.FirstOrDefaultAsync(c => c.Id.ToString() == id);
            }
        }

        public async Task UpdateConstraintAsync(ProblemConstraint constraint)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                context.ProblemConstraint.Update(constraint);
                await context.SaveChangesAsync();
            }
        }

        public async Task<List<ProblemConstraint>> GetConstraintsForProblemAsync(int problemId)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                return await context.ProblemConstraint
                    .Where(c => c.ProblemId == problemId)
                    .ToListAsync();
            }
        }
    }
}
