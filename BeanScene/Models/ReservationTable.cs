namespace BeanScene.Models
{
    public class ReservationTable
    {
        public int ReservationId { get; set; }

        public Reservation? Reservation { get; set; }

        public int TableId { get; set; }

        public Table? Table { get; set; }
    }
}