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
        public void Delete(TravelOffering offering)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TravelOffering> GetAll()
        {
            throw new NotImplementedException();
        }

        public TravelOffering GetById(int id)
        {
            throw new NotImplementedException();
        }

        public void Insert(TravelOffering offering)
        {
            throw new NotImplementedException();
        }

        public void save()
        {
            throw new NotImplementedException();
        }

        public void Update(TravelOffering offering)
        {
            throw new NotImplementedException();
        }
    }
}
