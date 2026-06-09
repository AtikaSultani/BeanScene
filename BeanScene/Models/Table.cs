using System.ComponentModel.DataAnnotations;

namespace BeanScene.Models
{
    public class Table
    {
        public int TableId { get; set; }

        [Required]
        public string TableCode { get; set; } = string.Empty;

        [Required]
        public int Capacity { get; set; }

        public int AreaId { get; set; }

        public Area? Area { get; set; }

        public ICollection<ReservationTable>? ReservationTables { get; set; }
    }
}