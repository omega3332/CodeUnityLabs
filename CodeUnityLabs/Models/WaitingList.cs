using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeUnityLabs.Models
{
    public class WaitingList
    {
        [Key]
        public int Waiting_Id { get; set; }

        public int User_Id { get; set; }
        public DateTime Requested_At { get; set; }
        public string? Status { get; set; }
        public int Priority { get; set; }

        [ForeignKey("User_Id")]
        public User? User { get; set; }
    }
}
