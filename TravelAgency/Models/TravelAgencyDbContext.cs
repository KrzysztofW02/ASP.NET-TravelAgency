﻿using Microsoft.EntityFrameworkCore;

namespace TravelAgency.Models
{
    public class TravelAgencyDbContext : DbContext
    {
        public TravelAgencyDbContext(DbContextOptions<TravelAgencyDbContext> options)
            : base(options)
        {
        }

        public DbSet<TravelOffering> TravelOfferings { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Transportation> Transportations { get; set; }
        public DbSet<Promotion> Promotions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClientTravelOffering>()
                .HasKey(cto => new { cto.ClientId, cto.TravelOfferingId });

            modelBuilder.Entity<ClientTravelOffering>()
                .HasOne(cto => cto.Client)
                .WithMany(c => c.ClientTravelOfferings)
                .HasForeignKey(cto => cto.ClientId);

            modelBuilder.Entity<ClientTravelOffering>()
                .HasOne(cto => cto.TravelOffering)
                .WithMany(t => t.ClientTravelOfferings)
                .HasForeignKey(cto => cto.TravelOfferingId);
        }
    }
}
