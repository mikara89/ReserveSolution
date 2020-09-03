using IdentityService.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityService.Data.Persistence
{
    public class IdentityDBContext : IdentityDbContext<AppUser>
    {
        public IdentityDBContext(DbContextOptions<IdentityDBContext> options)
            : base(options)
        {

        }
        public IdentityDBContext()
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=Pigeon_Carrier_IdentityService;Integrated Security=True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppUser>()
             .Property(e => e.IsActive)
             .HasDefaultValue(true);

            base.OnModelCreating(modelBuilder);
        }
    }
}
