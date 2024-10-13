namespace TravelAgency.ViewModels
{
    public class ManageUserPasswordViewModel
    {
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public int PasswordHistoryLimit { get; set; }
        public int PasswordExpirationDays { get; set; }
        public int PasswordNumbersRequired { get; set; }
        public int PasswordLengthRequired { get; set; }
        public bool IsPasswordChangeRequired { get; set; }
    }
}
