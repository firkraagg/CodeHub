using Microsoft.EntityFrameworkCore;
using CodeHub.Data;
using CodeHub.Data.Models;

namespace CodeHub.Services
{
    public class UserService
    {
        private IDbContextFactory<DatabaseContext> _dbContextFactory;
        public UserService(IDbContextFactory<DatabaseContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public void AddUser(User user)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                context.Users.Add(user);
                context.SaveChanges();
            }
        }

        public User? FindByUsername(string username)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                return context.Users.FirstOrDefault(u => u.Username == username);
            }
        }

        public User? FindByEmail(string email)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                return context.Users.FirstOrDefault(u => u.Email == email);
            }
        }
    }
}
