using Microsoft.EntityFrameworkCore;
using System;
using Player.Data.Models.Entites;

namespace Player.Data.Persistence
{
    public class PlayerDBContext : DbContext
    { 
        public DbSet<PlayerEntity> Players { get; set; }  
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
               .Property(e => e.IsActive)
               .HasDefaultValue(true);
            modelBuilder.Entity<PlayerEntity>()
               .Property(e => e.NickName)
               .IsRequired(true);
            modelBuilder.Entity<PlayerEntity>()
               .Property(e => e.TeamId)
               .IsRequired(true);
            modelBuilder.Entity<PlayerEntity>()
               .Property(e => e.UserId)
               .IsRequired(true);

            base.OnModelCreating(modelBuilder);
        }
    }
}
