﻿namespace TravelAgency.Models
{
    public class PasswordHistory
    {
        public int Id { get; set; }
        public string PasswordHash { get; set; }
        public DateTime DateChanged { get; set; }
        public string UserId { get; set; }
    }
}