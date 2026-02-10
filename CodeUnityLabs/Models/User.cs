using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeUnityLabs.Models
{
    public class User
    {
        [Key]
        public int User_Id { get; set; }

        [Required]
        public required string Name { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string Password { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Select User Type")]
        public int User_Type_Id { get; set; }

        public UserType? UserType { get; set; }

        public ICollection<Rezervation> Rezervations { get; set; } = new List<Rezervation>();
        public ICollection<Authorizations> Authorizations { get; set; } = new List<Authorizations>();
        public ICollection<WaitingList> WaitingLists { get; set; } = new List<WaitingList>();

    }
}
