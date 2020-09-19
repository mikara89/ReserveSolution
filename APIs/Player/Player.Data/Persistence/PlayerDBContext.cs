using Microsoft.EntityFrameworkCore;
using System;
using Player.Data.Models.Entites;

namespace Player.Data.Persistence
{
    public class PlayerDBContext : DbContext
    { 
        public DbSet<PlayerEntity> Players { get; set; }
        public DbSet<InvetationEntity> Invetations { get; set; } 
        public PlayerDBContext(DbContextOptions options)
            : base(options)
        {

        }
        public PlayerDBContext()
        {
          
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=TeamPlayer;Integrated Security=True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlayerEntity>()
               .Property(e => e.CreatedAt)
               .HasDefaultValue(DateTime.Now); 
            
            modelBuilder.Entity<PlayerEntity>()
               .Property(e => e.UserId)
               .HasMaxLength(36)
               .IsRequired(true);

            modelBuilder.Entity<PlayerInfoEntity>()
               .Property(e => e.IsActive)
               .HasDefaultValue(true);
            modelBuilder.Entity<PlayerInfoEntity>()
               .Property(e => e.FullName)
               .HasMaxLength(50)
               .IsRequired(true);
            modelBuilder.Entity<PlayerInfoEntity>()
               .Property(e => e.NickName)
               .HasMaxLength(50)
               .IsRequired(true);
            modelBuilder.Entity<PlayerInfoEntity>()
               .Property(e => e.TeamId)
               .IsRequired(true);
            modelBuilder.Entity<PlayerInfoEntity>()
              .Property(e => e.UpdatedAt)
              .HasDefaultValue(DateTime.Now);

            modelBuilder.Entity<InvetationEntity>()
             .Property(e => e.Expiration)
             .IsRequired();
            modelBuilder.Entity<InvetationEntity>()
             .Property(e => e.TeamId)
             .HasMaxLength(36)
             .IsRequired();
            modelBuilder.Entity<InvetationEntity>()
             .Property(e => e.IsCanceled)
             .HasDefaultValue(false)
             .IsRequired();
            modelBuilder.Entity<InvetationEntity>()
            .Property(e => e.IsUsed)
            .HasDefaultValue(false);

            base.OnModelCreating(modelBuilder);
        }
    }
}
