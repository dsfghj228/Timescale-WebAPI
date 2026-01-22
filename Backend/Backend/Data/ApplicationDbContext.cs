using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {}
    
    public DbSet<FileImport> Files { get; set; }
    public DbSet<ResultAggregate> Results { get; set; }
    public DbSet<ValueEntry> Values { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<FileImport>()
            .HasMany(f => f.Values)
            .WithOne(v => v.FileImport)
            .HasForeignKey(v => v.FileImportId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<FileImport>()
            .HasOne(f => f.Result)
            .WithOne(r => r.FileImport)
            .HasForeignKey<ResultAggregate>(r => r.FileImportId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<FileImport>()
            .HasIndex(f => f.FileName).IsUnique();
        
        modelBuilder.Entity<ValueEntry>()
            .HasIndex(v => new { v.FileImportId, v.Timestamp });
        
        modelBuilder.Entity<ResultAggregate>()
            .HasIndex(r => r.FirstOperationDate);
        
        modelBuilder.Entity<ResultAggregate>()
            .HasIndex(r => r.AverageValue);
        
        modelBuilder.Entity<ResultAggregate>()
            .HasIndex(r => r.AverageExecutionTime);
    }
}