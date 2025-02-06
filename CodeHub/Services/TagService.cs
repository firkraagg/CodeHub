using CodeHub.Data.Models;
using CodeHub.Data;
using CodeHub.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CodeHub.Services
{
    public class TagService
    {
        private readonly IDbContextFactory<DatabaseContext> _dbContextFactory;

        public TagService(IDbContextFactory<DatabaseContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<List<Tag>> GetTagsAsync()
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                return await context.Tag.ToListAsync();
            }
        }

        public async Task<List<Problem>> GetProblemsByTagAsync(string tagName)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                var tag = await context.Tag
                    .FirstOrDefaultAsync(t => t.Name.Equals(tagName, StringComparison.OrdinalIgnoreCase));

                if (tag == null)
                {
                    return new List<Problem>();
                }

                var problems = await context.Problems
                    .Where(p => p.Tags.Any(t => t.Name == tagName))
                    .ToListAsync();

                return problems;
            }
        }

        public async Task<List<Problem>> GetProblemsByTagsAsync(List<string> tagNames)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                var tags = await context.Tag
                    .Where(t => tagNames.Contains(t.Name))
                    .ToListAsync();

                if (!tags.Any())
                {
                    return new List<Problem>();
                }

                var problems = await context.Problems
                    .Where(p => p.Tags.Any(t => tags.Contains(t)))
                    .ToListAsync();

                return problems;
            }
        }

        public async Task<List<Tag>> GetTagsForProblemAsync(int problemId)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                var problem = await context.Problems
                    .Include(p => p.Tags)
                    .FirstOrDefaultAsync(p => p.Id == problemId);

                return problem?.Tags.ToList() ?? new List<Tag>();
            }
        }
    }
}
