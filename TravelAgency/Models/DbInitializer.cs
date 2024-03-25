using System;
using System.Linq;
using TravelAgency.Models;

namespace TravelAgency.Data
{
    public static class DbInitializer
    {
        public static void Initialize(TravelAgencyDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Clients.Any() || context.TravelOfferings.Any() || context.Employees.Any() || context.Transportations.Any() || context.Promotions.Any())
            {
                return;  
            }

            var clients = new Client[]
            {
                new Client { FirstName = "John", LastName = "Doe", Number = "123456789", Email = "john@gmail.com" },
                new Client { FirstName = "Jane", LastName = "Smith", Number = "987654321", Email = "jane@gmail.com" }
            };
            foreach (var client in clients)
            {
                context.Clients.Add(client);
            }
            context.SaveChanges();

            var travelOfferings = new TravelOffering[]
            {
                new TravelOffering { Name = "France Adventure", Destination = "Paris, France", StartDate = DateTime.Parse("2024-06-01"), EndDate = DateTime.Parse("2024-06-15"), Price = 2500.00m, Description = "Enjoy a relaxing vacation in the beautiful Paris." },
                new TravelOffering { Name = "Italy Adventure", Destination = "Rome, Italy", StartDate = DateTime.Parse("2024-07-10"), EndDate = DateTime.Parse("2024-07-25"), Price = 3500.00m, Description = "Enjoy a relaxing vacation in the beautiful Rome" }
            };
            foreach (var offering in travelOfferings)
            {
                context.TravelOfferings.Add(offering);
            }
            context.SaveChanges();

            var employees = new Employee[]
            {
                new Employee { FirstName = "Emily", LastName = "Johnson", Position = "Travel Agent", Email = "emily@gmail.com", Salary = 5000.00m }
            };
            foreach (var employee in employees)
            {
                context.Employees.Add(employee);
            }
            context.SaveChanges();

            var transportations = new Transportation[]
            {
                new Transportation { Type = "Flight", Details = "Flight from Kraków to Paris" },
                new Transportation { Type = "Train", Details = "Train from Warsaw to Rome" }
            };
            foreach (var transportation in transportations)
            {
                context.Transportations.Add(transportation);
            }
            context.SaveChanges();

            var promotions = new Promotion[]
            {
                new Promotion { Name = "Free Discount", Description = "Book early and get 10% off", StartDate = DateTime.Parse("2024-01-01"), EndDate = DateTime.Parse("2024-06-30"), Discount = 10.00m }
            };
            foreach (var promotion in promotions)
            {
                context.Promotions.Add(promotion);
            }
            context.SaveChanges();
        }
    }
}
