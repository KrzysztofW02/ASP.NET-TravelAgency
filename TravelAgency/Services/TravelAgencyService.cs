using Microsoft.EntityFrameworkCore;
using TravelAgency.Models;
using TravelAgency.Repository;

namespace TravelAgency.Services
{
    public class TravelAgencyService : ITravelAgencyService
    {
        private readonly ITravelOfferingsRepository _travelOfferingsRepository;

        public TravelAgencyService(ITravelOfferingsRepository travelOfferingsRepository)
        {
            _travelOfferingsRepository = travelOfferingsRepository;
        }

        public async Task<IEnumerable<TravelOffering>> GetAllAsync()
        {
            return await _travelOfferingsRepository.GetAllAsync();
        }

        public async Task<TravelOffering> GetByIdAsync(int id)
        {
            return await _travelOfferingsRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(TravelOffering travelOffering)
        {
            await _travelOfferingsRepository.AddAsync(travelOffering);
        }

        public async Task UpdateAsync(TravelOffering travelOffering)
        {
            await _travelOfferingsRepository.UpdateAsync(travelOffering);
        }

        public async Task DeleteAsync(int id)
        {
            await _travelOfferingsRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<TravelOffering>> SearchAsync(string searchString)
        {
            var offerings = await _travelOfferingsRepository.GetAllAsync();
            return offerings.Where(o => o.Name.Contains(searchString) || o.Destination.Contains(searchString));
        }

        public async Task<IEnumerable<TravelOffering>> SortAsync(string sortOrder)
        {
            var offerings = await _travelOfferingsRepository.GetAllAsync();

            switch (sortOrder)
            {
                case "name_desc":
                    return offerings.OrderByDescending(o => o.Name);
                case "Date":
                    return offerings.OrderBy(o => o.StartDate);
                case "date_desc":
                    return offerings.OrderByDescending(o => o.StartDate);
                default:
                    return offerings.OrderBy(o => o.Name);
            }
        }
    }
}
