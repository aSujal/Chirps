using Chirps.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;

namespace Chirps;

public class DataContext : DbContext
{
    // Define DbSets for your entities
    public DbSet<User> Users { get; set; }
    public DbSet<Chirp> Chirps { get; set; }
    public DbSet<Peep> Peeps { get; set; }
    public DbSet<PeepChirp> PeepChirps { get; set; }
    public DbSet<Like> Likes { get; set; }

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
            entity.HasIndex(u => u.Email)
            .IsUnique();
        });

        modelBuilder.Entity<Peep>()
            .HasIndex(p => p.Text)
            .IsUnique();

        modelBuilder.Entity<PeepChirp>(entity =>
        {
            entity.HasIndex(pc => new { pc.PeepId, pc.ChirpId })
                .IsUnique();
            entity.HasKey(pc => pc.Id);
            entity.HasOne(pc => pc.Peep)
                .WithMany(p => p.PeepChirps)
                .HasForeignKey(pc => pc.PeepId);
            entity.HasOne(pc => pc.Chirp)
                .WithMany(c => c.PeepChirps)
                .HasForeignKey(pc => pc.ChirpId);
        });

        modelBuilder.Entity<Chirp>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Content)
                .IsRequired()
                .HasMaxLength(123);
            entity.Property(c => c.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
            entity.HasOne(c => c.User)
                .WithMany(u => u.Chirps)
                .HasForeignKey(c => c.UserId);
        });
        modelBuilder.Entity<Like>()
            .HasIndex(l => new { l.UserId, l.ChirpId })
            .IsUnique();

        modelBuilder.Entity<Like>()
        .HasOne(l => l.User)
        .WithMany(u => u.LikesGiven)
        .HasForeignKey(l => l.UserId)
        .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Like>()
            .HasOne(l => l.Chirp)
            .WithMany(c => c.Likes)
            .HasForeignKey(l => l.ChirpId)
            .OnDelete(DeleteBehavior.Cascade);

    }

}
