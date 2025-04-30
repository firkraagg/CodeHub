using CodeHub.Data.Entities;
using CodeHub.Data;
using Microsoft.EntityFrameworkCore;

namespace CodeHub.Services
{
    public class VisibleWeekService
    {
        private readonly IDbContextFactory<DatabaseContext> _dbContextFactory;

        public VisibleWeekService(IDbContextFactory<DatabaseContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<List<int>> GetVisibleWeeksAsync()
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                return await context.VisibleWeeks
                    .Select(vw => vw.NumberOfWeek)
                    .ToListAsync();
            }
        }

        public async Task UpdateVisibleWeeksAsync(List<int> selectedWeeks)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                var existingWeeks = await context.VisibleWeeks.ToListAsync();
                context.VisibleWeeks.RemoveRange(existingWeeks);

                var newVisibleWeeks = selectedWeeks
                    .Select(weekNumber => new VisibleWeek { NumberOfWeek = weekNumber })
                    .ToList();

                await context.VisibleWeeks.AddRangeAsync(newVisibleWeeks);
                await context.SaveChangesAsync();
            }
        }
    }
}
