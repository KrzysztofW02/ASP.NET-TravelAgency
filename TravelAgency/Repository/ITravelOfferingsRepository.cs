using TravelAgency.Models;

namespace TravelAgency.Repository
{
    public interface ITravelOfferingsRepository
    {
        Task AddAsync(TravelOffering travelOffering);
        Task DeleteAsync(int id);
        Task<IQueryable<TravelOffering>> GetAllAsync();
        Task<TravelOffering> GetByIdAsync(int id);
        Task UpdateAsync(TravelOffering travelOffering);
    }
}