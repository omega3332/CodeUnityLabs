using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeUnityLabs.Models
{
    public class WaitingList
    {
        [Key]
        public int Waiting_Id { get; set; }

        [Required(ErrorMessage = "User is required")]
        public int User_Id { get; set; }

        [Required(ErrorMessage = "Requested date/time is required")]
        public DateTime Requested_At { get; set; }

        [Required]
        [RegularExpression("Active|Inactive", ErrorMessage = "Status must be Active or Inactive")]
        public string Status { get; set; } = "Active";

        [Required]
        [Range(1, 100, ErrorMessage = "Priority must be between 1 and 100")]
        public int Priority { get; set; }

        [ForeignKey("User_Id")]
        public User? User { get; set; }
    }
}
