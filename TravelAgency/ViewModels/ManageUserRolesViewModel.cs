namespace TravelAgency.ViewModels
{
    public class ManageUserRolesViewModel
    {
        public string UserId { get; set; }
        public IEnumerable<string> UserRoles { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public IEnumerable<string> SelectedRoles { get; set; }
    }

}
