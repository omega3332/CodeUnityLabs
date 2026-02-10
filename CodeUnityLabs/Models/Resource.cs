using System.ComponentModel.DataAnnotations;
namespace CodeUnityLabs.Models
{
    public class Resource
    {
        [Key] 
        public int Resource_Id { get; set; }

        [Required(ErrorMessage = "Resource Name is required")]
        public string Resource_Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Resource Type is required")]
        public string Resource_Type { get; set; } = string.Empty;

        public bool Available { get; set; }

        public ICollection<Rezervation>? Reservations { get; set; }
    }
}
