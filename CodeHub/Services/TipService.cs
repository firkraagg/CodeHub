using CodeHub.Data;
using CodeHub.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CodeHub.Services
{
    public class TipService
    {
        private readonly IDbContextFactory<DatabaseContext> _dbContextFactory;

        public TipService(IDbContextFactory<DatabaseContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<List<Tip>> GetTipsAsync()
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                return await context.Tip.ToListAsync();
            }
        }

        public async Task<Tip> GetRandomTipAsync()
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                var tips = await GetTipsAsync();
                var random = new Random();
                return tips[random.Next(tips.Count)];
            }
        }
    }
}
