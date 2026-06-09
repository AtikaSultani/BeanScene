using Microsoft.AspNetCore.Identity;
namespace BeanScene.Data;
// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public string? PhotoPath { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
}
