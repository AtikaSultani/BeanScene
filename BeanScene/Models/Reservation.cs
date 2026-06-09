using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeanScene.Models
{
    public class Reservation
    {
        public int ReservationId { get; set; }

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public DateTime? ReservationDate { get; set; }

        [Range(1, 20, ErrorMessage = "Number of guests must be between 1 and 20.")]
        public int NumberOfGuests { get; set; }

        [Required]
        public TimeOnly StartTime { get; set; }

        [Required]
        public int DurationMinutes { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Pending";

        public string? Notes { get; set; }

        [Required]
        public string Source { get; set; } = "Online";

        // Foreign Key
        public int SittingId { get; set; }

        public Sitting? Sitting { get; set; }

        // Member account (optional)
        public string? UserId { get; set; }
        [NotMapped]
        public List<Area> Areas { get; set; } = new();

        [NotMapped]
        public List<Table> Tables { get; set; } = new();

        [NotMapped]
        public List<Sitting> Sittings { get; set; } = new();

        [NotMapped]
        public int? SelectedAreaId { get; set; }

        [Required(ErrorMessage = "Please select a table.")]
        [NotMapped] 
        public int? SelectedTableId { get; set; }





        public ICollection<ReservationTable>? ReservationTables { get; set; }
    }
}