namespace TravelAgency.Models
{
    public class UserActivityLog
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string UserEmail{ get; set; }
        public string Activity { get; set; }
        public DateTime ActivityTime { get; set; }
        public UserActivityLog(string userId, string userEmail, string activity)
        {
            Id = Guid.NewGuid().ToString();
            UserId = userId;
            UserEmail = userEmail;
            Activity = activity;
            ActivityTime = DateTime.Now;
        }
    }
}
