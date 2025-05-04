using CodeHub.Data;
using CodeHub.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CodeHub.Services
{
    public class TestCaseService
    {
        private readonly IDbContextFactory<DatabaseContext> _dbContextFactory;

        public TestCaseService(IDbContextFactory<DatabaseContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<TestCase?> GetTestCaseByIdAsync(string id)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                return await context.TestCases.FirstOrDefaultAsync(t => t.Id.ToString() == id);
            }
        }

        public async Task UpdateTestCaseAsync(TestCase testCase)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                context.TestCases.Update(testCase);
                await context.SaveChangesAsync();
            }
        }

        public async Task<List<TestCase>> GetTestCasesForProblemAsync(int problemId)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                return await context.TestCases
                    .Where(t => t.ProblemId == problemId)
                    .ToListAsync();
            }
        }
    }
}