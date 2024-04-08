namespace TravelAgency.ViewModels
{
    public class TravelOfferingsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Destination { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
    }
}