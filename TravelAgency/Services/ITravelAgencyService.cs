using TravelAgency.Models;
using TravelAgency.ViewModels;

namespace TravelAgency.Services
{
    public interface ITravelAgencyService
    {
        Task AddAsync(TravelOffering travelOffering);
        Task DeleteAsync(int id);
        Task<IEnumerable<TravelOffering>> GetAllAsync();
        Task<TravelOffering> GetByIdAsync(int id);
        Task UpdateAsync(TravelOffering travelOffering);
        Task<IEnumerable<TravelOffering>> SearchAsync(string searchString);
        Task<IEnumerable<TravelOffering>> SortAsync(string sortOrder);
    }
}