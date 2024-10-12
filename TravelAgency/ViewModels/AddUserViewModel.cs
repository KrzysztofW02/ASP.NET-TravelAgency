using System.ComponentModel.DataAnnotations;

namespace TravelAgency.ViewModels
{
    public class AddUserViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

}
