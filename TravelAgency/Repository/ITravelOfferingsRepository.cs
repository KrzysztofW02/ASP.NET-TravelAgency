using TravelAgency.Models;

namespace TravelAgency.Repository
{
    public interface ITravelOfferingsRepository
    {
        IEnumerable<TravelOffering> GetAll();
        TravelOffering GetById(int id);
        void Insert(TravelOffering offering);
        void Update(TravelOffering offering);
        void Delete(TravelOffering offering);
        void save();
    }
}
