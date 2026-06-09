using BeanScene.Models;
using System.ComponentModel.DataAnnotations;

namespace BeanScene.ViewModels
{
    public class ReservationViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Reservation Date")]
        public DateTime ReservationDate { get; set; }

        [Required]
        [Display(Name = "Number of Guests")]
        public int NumberOfGuests { get; set; }

        [Required]
        [Display(Name = "Start Time")]
        public TimeOnly StartTime { get; set; }

        [Required]
        [Display(Name = "Duration (Minutes)")]
        public int DurationMinutes { get; set; }

        [Required]
        [Display(Name = "Sitting")]
        public int SittingId { get; set; }

        public List<Area> Areas { get; set; } = new();

        public List<Table> Tables { get; set; } = new();

        [Required(ErrorMessage = "Please select a table.")]
        public int? SelectedTableId { get; set; }
        public int? SelectedAreaId { get; set; }

        [Display(Name = "Special Requests")]
        public string? Notes { get; set; }

        public List<Sitting> Sittings { get; set; } = new();
    }
}