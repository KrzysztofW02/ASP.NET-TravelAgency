namespace TravelAgency.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Number { get; set; }
        public string Email { get; set; }

        public virtual ICollection<ClientTravelOffering> ClientTravelOfferings { get; set; }
    }
}
