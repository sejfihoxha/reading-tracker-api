using Microsoft.EntityFrameworkCore;
using ReadingTracker.Domain.Entities;

namespace ReadingTracker.Infrastructure.Data;

public class ReadingTrackerDbContext : DbContext
{
    public ReadingTrackerDbContext(DbContextOptions<ReadingTrackerDbContext> options)
        : base(options)
    {
    }

    public DbSet<Book> Books => Set<Book>();
    public DbSet<ReadingSession> ReadingSessions => Set<ReadingSession>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Book entity
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired();
            entity.Property(e => e.Author).IsRequired();
            entity.Property(e => e.Year).IsRequired();
            entity.Property(e => e.Pages).IsRequired();
            
            // Store Genres as JSON (SQLite compatible)
            entity.Property(e => e.Genres)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                );
        });

        // Configure ReadingSession entity
        modelBuilder.Entity<ReadingSession>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BookId).IsRequired();
            entity.Property(e => e.StartedAt).IsRequired();
            
            // Create index on BookId for better query performance
            entity.HasIndex(e => e.BookId);
        });
    }
}
