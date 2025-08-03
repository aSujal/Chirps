using ExampleBlazor.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;

namespace ExampleBlazor;

public class DataContext : DbContext
{
    // Define DbSets for your entities
    public DbSet<User> Users { get; set; }
    public DbSet<Article> Articles { get; set; }
    public DbSet<StockTransaction> StockTransactions { get; set; }

    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Username)
            .IsUnique();
        });

        modelBuilder.Entity<Article>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.HasIndex(a => a.ArticleNumber)
                .IsUnique();
            entity.Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(a => a.Description)
                .HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        modelBuilder.Entity<StockTransaction>(entity =>
        {
            entity.HasKey(st => st.Id);
            entity.Property(st => st.Notes)
                .HasMaxLength(500);
            entity.Property(st => st.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(st => st.Date).HasDefaultValueSql("GETUTCDATE()");
            entity.HasOne(st => st.Article)
                .WithMany(a => a.Transactions)
                .HasForeignKey(st => st.ArticleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // examples

        //modelBuilder.Entity<Ticket>(entity =>
        //{
        //    entity.HasKey(t => t.Id);
        //    entity.HasOne(t => t.Sprint)
        //        .WithMany(s => s.Tickets)
        //        .HasForeignKey(t => t.SprintId)
        //        .OnDelete(DeleteBehavior.SetNull);
        //    entity.HasOne(t => t.User)
        //        .WithMany(u => u.Tickets)
        //        .HasForeignKey(t => t.UserId)
        //        .OnDelete(DeleteBehavior.SetNull);
        //});

        //modelBuilder.Entity<Sprint>(entity =>
        //{
        //    entity.HasKey(s => s.Id);

        //    entity.HasOne(s => s.User)
        //        .WithMany(u => u.Sprints)
        //        .HasForeignKey(s => s.UserId)
        //        .OnDelete(DeleteBehavior.SetNull);

        //    entity.HasMany(s => s.Tickets)
        //        .WithOne(t => t.Sprint)
        //        .HasForeignKey(t => t.SprintId);
        //});
    }

}
