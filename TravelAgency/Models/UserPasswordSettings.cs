namespace TravelAgency.Models
{
    public class UserPasswordSettings
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int PasswordHistoryLimit { get; set; }
        public int PasswordExpirationDays { get; set; }
        public bool IsPasswordChangeRequired { get; set; }
        public int PasswordNumbersRequired { get; set; }
        public int PasswordLengthRequired { get; set; }
        public bool OneTimePasswordActive { get; set; }

    }
}
