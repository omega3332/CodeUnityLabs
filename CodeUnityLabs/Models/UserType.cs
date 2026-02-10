using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeUnityLabs.Models
{
    [Table("UserType")]
    public class UserType
    {
        [Key]
        public int User_Type_Id { get; set; }
        public string? Type_Name { get; set; }
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
