using CodeHub.Data;
using CodeHub.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CodeHub.Services
{
    public class ProgrammingLanguageService
    {
        private readonly IDbContextFactory<DatabaseContext> _dbContextFactory;

        public ProgrammingLanguageService(IDbContextFactory<DatabaseContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<List<ProgrammingLanguage>> GetProgrammingLanguagesAsync()
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                return await context.ProgrammingLanguages.ToListAsync();
            }
        }   
    }
}
