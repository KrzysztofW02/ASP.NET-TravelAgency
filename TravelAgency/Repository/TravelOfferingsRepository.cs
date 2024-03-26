using Microsoft.EntityFrameworkCore;
using TravelAgency.Models;
namespace TravelAgency.Repository
{
    public class TravelOfferingsRepository : ITravelOfferingsRepository
    {
        private readonly TravelAgencyDbContext _context;

        public TravelOfferingsRepository(TravelAgencyDbContext context)
        {
            _context = context;
        }

        public async Task<IQueryable<TravelOffering>> GetAllAsync()
        {
            return _context.TravelOfferings.AsQueryable();
        }

        public async Task<TravelOffering> GetByIdAsync(int id)
        {
            return await _context.TravelOfferings.FindAsync(id);
        }

        public async Task AddAsync(TravelOffering travelOffering)
        {
            _context.Add(travelOffering);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TravelOffering travelOffering)
        {
            _context.Update(travelOffering);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var travelOffering = await _context.TravelOfferings.FindAsync(id);
            if (travelOffering != null)
            {
                _context.TravelOfferings.Remove(travelOffering);
                await _context.SaveChangesAsync();
            }
        }
    }
}
