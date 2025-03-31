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
            this.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Problem> Problems { get; set; }
        public DbSet<ProgrammingLanguage> ProgrammingLanguage { get; set; }
        public DbSet<Tag> Tag { get; set; }
        public DbSet<ProblemHint> ProblemHint { get; set; }
        public DbSet<ProblemConstraint> ProblemConstraint { get; set; }
        public DbSet<ProblemExample> ProblemExample { get; set; }
        public DbSet<TestCase> TestCase { get; set; }
        public DbSet<ProblemAttempt> ProblemAttempts { get; set; }

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

            modelBuilder.Entity<Problem>()
                .HasMany(p => p.Tags)
                .WithMany(t => t.Problems)
                .UsingEntity<Dictionary<string, object>>(
                    "ProblemTag",
                    j => j.HasOne<Tag>().WithMany().HasForeignKey("TagId").OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne<Problem>().WithMany().HasForeignKey("ProblemId").OnDelete(DeleteBehavior.Cascade)
                );

            modelBuilder.Entity<ProblemAttempt>()
                .HasKey(pa => pa.Id);

            modelBuilder.Entity<ProblemAttempt>()
                .HasOne(pa => pa.User)
                .WithMany(u => u.SolvedProblems)
                .HasForeignKey(pa => pa.userId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProblemAttempt>()
                .HasOne(pa => pa.Problem)
                .WithMany(p => p.SolvedByUsers)
                .HasForeignKey(pa => pa.problemId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
