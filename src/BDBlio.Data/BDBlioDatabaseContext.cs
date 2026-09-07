using BDBlio.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace BDBlio.Data;

public class BDBlioDatabaseContext : DbContext
{
    public DbSet<ComicBook> ComicBooks { get; set; } = null!;
    public DbSet<Rating> Ratings { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var dataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "BDBlio"
        );

        if (!Directory.Exists(dataPath))
            Directory.CreateDirectory(dataPath);

        var dbPath = Path.Combine(dataPath, "BDBlio.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuration de la relation ComicBook -> Ratings
        modelBuilder.Entity<ComicBook>()
            .HasMany(c => c.Ratings)
            .WithOne(r => r.ComicBook)
            .HasForeignKey(r => r.ComicBookId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index pour optimiser les recherches
        modelBuilder.Entity<ComicBook>()
            .HasIndex(c => c.Title);
        modelBuilder.Entity<ComicBook>()
            .HasIndex(c => c.Author);
        modelBuilder.Entity<ComicBook>()
            .HasIndex(c => c.ISBN);
        modelBuilder.Entity<ComicBook>()
            .HasIndex(c => c.EAN);
        modelBuilder.Entity<ComicBook>()
            .HasIndex(c => c.Collection);
    }
}
