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
    }
}
