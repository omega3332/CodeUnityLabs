using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CodeUnityLabs.Models; 

namespace CodeUnityLabs.Models
{
    public class Rezervation
    {

        [Key]
        [Column("reservation_id")]
        public int Reservation_Id { get; set; }

        [Column("user_id")]
        public int User_Id { get; set; }

        [Column("resource_id")]
        public int Resource_Id { get; set; }

        [ForeignKey("Resource_Id")]
        public Resource? Resource { get; set; }

        [Column("equipment_id")]
        public int? Equipment_Id { get; set; }

        [Column("room_id")]
        public int? Room_Id { get; set; }

        [Column("start_time")]
        public DateTime Start_Time { get; set; }

        [Column("end_time")]
        public DateTime End_Time { get; set; }

        public ReservationStatus Status { get; set; }

        [Column("priority")]
        public int Priority { get; set; }

        [ForeignKey("User_Id")]
        public User? User { get; set; }

        // Add navigation for Resource, Equipment, Room if I have models
    }
}
