using System.ComponentModel.DataAnnotations;

namespace BeanScene.ViewModels
{
    public class ContactViewModel
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Message { get; set; } = string.Empty;
    }
}