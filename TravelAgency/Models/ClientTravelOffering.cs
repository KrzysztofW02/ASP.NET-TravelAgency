namespace TravelAgency.Models
{
    public class ClientTravelOffering
    {
        public int ClientId { get; set; }
        public Client Client { get; set; }

        public int TravelOfferingId { get; set; }
        public TravelOffering TravelOffering { get; set; }
    }
}

// data rezerwacji, koszt nullable Koszt ostateczny(całkowity)