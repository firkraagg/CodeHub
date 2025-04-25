using CodeHub.Data;
using CodeHub.Data.Entities;
using CodeHub.Data.Models;
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
                    .Where(p => p.Tags.Count(t => tagNames.Contains(t.Name)) == tagNames.Count)
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

        public async Task<List<string>> GetTagNamesForProblemAsync(int problemId)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                var tagNames = await context.Problems
                    .Where(p => p.Id == problemId)
                    .SelectMany(p => p.Tags.Select(t => t.Name))
                    .ToListAsync();

                return tagNames;
            }
        }

        public async Task<bool> DeleteTagAsync(string tagName)
        {
            using var context = _dbContextFactory.CreateDbContext();

            var tagsToDelete = await context.Tag
                .Where(t => t.Name == tagName)
                .Include(t => t.Problems)
                .ToListAsync();

            if (!tagsToDelete.Any())
                return false;

            foreach (var tag in tagsToDelete)
            {
                tag.Problems?.Clear();
            }

            context.Tag.RemoveRange(tagsToDelete);
            await context.SaveChangesAsync();

            return true;
        }
    }
}
