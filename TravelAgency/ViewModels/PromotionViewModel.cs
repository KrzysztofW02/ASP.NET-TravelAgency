﻿namespace TravelAgency.ViewModels
{
    public class PromotionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Discount { get; set; }
    }
}
