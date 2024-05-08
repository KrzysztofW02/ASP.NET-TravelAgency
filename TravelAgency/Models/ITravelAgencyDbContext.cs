using Microsoft.EntityFrameworkCore;

namespace TravelAgency.Models
{
    public interface ITravelAgencyDbContext
    {
        DbSet<Client> Clients { get; set; }
        DbSet<Employee> Employees { get; set; }
        DbSet<Promotion> Promotions { get; set; }
        DbSet<Transportation> Transportations { get; set; }
        DbSet<TravelOffering> TravelOfferings { get; set; }
    }
}