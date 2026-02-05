using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeUnityLabs.Models
{
        [Table("UserType")] 
        public class UserType
        {
            [Key]
            [Column("user_type_id")]
            public int User_Type_Id { get; set; }

            [Column("type_name")]
            public string Type_Name { get; set; } = null!;
        }
    
}
