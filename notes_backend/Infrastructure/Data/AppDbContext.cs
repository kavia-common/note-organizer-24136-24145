using Microsoft.EntityFrameworkCore;
using NotesBackend.Domain.Entities;

namespace NotesBackend.Infrastructure.Data
{
    /// <summary>
    /// EF Core DbContext for Notes application.
    /// </summary>
    public class AppDbContext : DbContext
    {
        public const string DefaultSchema = "notes";

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Note> Notes => Set<Note>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema(DefaultSchema);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Note>()
                .HasIndex(n => new { n.UserId, n.CreatedAtUtc });

            modelBuilder.Entity<User>()
                .HasMany(u => u.Notes)
                .WithOne(n => n.User!)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
