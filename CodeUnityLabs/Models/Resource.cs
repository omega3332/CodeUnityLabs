using System.ComponentModel.DataAnnotations;
namespace CodeUnityLabs.Models
{
    public class Resource
    {
        [Key] 
        public int Resource_Id { get; set; }

        public string? Resource_Name{ get; set; }
        public string? Resource_Type { get; set; }
        public bool Available { get; set; }

        public ICollection<Rezervation>? Reservations { get; set; }
    }
}
