using System.ComponentModel.DataAnnotations;

namespace BeanScene.Models
{
    public class Sitting
    {
        public int SittingId { get; set; }

        [Required]
        [StringLength(255)]
        public string SittingType { get; set; } = string.Empty;

        [Required]
        public TimeOnly StartTime { get; set; }

        [Required]
        public TimeOnly EndTime { get; set; }

        [Required]
        public int Capacity { get; set; }
        public bool IsClosed { get; set; } = false;

        public ICollection<Reservation>? Reservations { get; set; }
    }
}