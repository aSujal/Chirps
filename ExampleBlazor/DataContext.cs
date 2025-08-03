using ExampleBlazor.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;

namespace ExampleBlazor;

public class DataContext : DbContext
{
    // Define DbSets for your entities
    public DbSet<User> Users { get; set; }
    // public DbSet<Ticket> Tickets { get; set; }

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
