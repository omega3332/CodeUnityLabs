using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
namespace CodeUnityLabs.Models
{
    public class User
    {
        [Key]
        public int User_Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        [Required(ErrorMessage = "User Type is required")]
        public int User_Type_Id { get; set; }

        // Navigation property
        public UserType UserType { get; set; }
        public ICollection<Authorizations>? Authorizations { get; set; }
        public ICollection<WaitingList>? WaitingLists { get; set; }
        public ICollection<Rezervation>? Reservations { get; set; }
    }
}
