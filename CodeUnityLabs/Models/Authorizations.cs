using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeUnityLabs.Models
{
    public class Authorizations
    {
        [Key]
        public int Authorization_Id { get; set; }

        public int User_Id { get; set; }
        public required string Permission { get; set; }
        public int? Granted_By { get; set; }
        public DateTime Granted_At { get; set; }
        public DateTime? Expires_At { get; set; }
        public int Priority { get; set; }

        [ForeignKey("User_Id")]
        public required User User { get; set; }
    }
}
