using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;


namespace BeanScene.ViewModels
{
    public class AddEmployeeViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty;
        public IFormFile? Photo { get; set; }
    }
}