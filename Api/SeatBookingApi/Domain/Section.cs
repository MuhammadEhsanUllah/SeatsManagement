using System.ComponentModel.DataAnnotations;

namespace SeatBookingApi.Domain
{
    public class Section:BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int RowsCount { get; set; }
        public int ColumnsCount { get; set; }
        // Navigation property
        public ICollection<Seat> Seats { get; set; } // Navigation property for seats
    }
}
