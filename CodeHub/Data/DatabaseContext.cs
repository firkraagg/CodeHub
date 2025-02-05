using CodeHub.Data.Entities;
using CodeHub.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeHub.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Problem> Problems { get; set; }
        public DbSet<ProgrammingLanguage> ProgrammingLanguage { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.Problems)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Problem>()
                .HasOne(p => p.ProgrammingLanguage)
                .WithMany()
                .HasForeignKey(p => p.LanguageID)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
