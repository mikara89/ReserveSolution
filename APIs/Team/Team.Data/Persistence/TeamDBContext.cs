using Microsoft.EntityFrameworkCore;
using System;
using Team.Data.Models.Entites;

namespace Team.Data.Persistence
{
    public class TeamDBContext : DbContext
    { 
        public DbSet<Models.Entites.TeamEntity> Teams { get; set; } 
        public TeamDBContext(DbContextOptions options)
            : base(options)
        {

        }
        public TeamDBContext()
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=Pigeon_Carrier_MailService;Integrated Security=True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Team.Data.Models.Entites.TeamEntity>()
               .Property(e => e.CreatedAt)
               .HasDefaultValue(DateTime.Now);
            modelBuilder.Entity<Team.Data.Models.Entites.TeamEntity>()
               .Property(e => e.IsActive)
               .HasDefaultValue(true);
            modelBuilder.Entity<Team.Data.Models.Entites.TeamEntity>()
               .Property(e => e.RegNumber)
               .IsRequired(true);
            modelBuilder.Entity<Team.Data.Models.Entites.TeamEntity>()
               .Property(e => e.TeamName)
               .IsRequired(true);
            modelBuilder.Entity<Team.Data.Models.Entites.TeamEntity>()
               .Property(e => e.UserId)
               .IsRequired(true);

            base.OnModelCreating(modelBuilder);
        }
    }
}
