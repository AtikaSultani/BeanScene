using System.ComponentModel.DataAnnotations;

namespace BeanScene.Models
{
    public class Area
    {
        public int AreaId { get; set; }

        [Required]
        [StringLength(255)]
        public string AreaName { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Description { get; set; }

        public ICollection<Table>? Tables { get; set; }
    }
}