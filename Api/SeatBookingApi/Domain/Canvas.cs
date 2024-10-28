using System.ComponentModel.DataAnnotations;

namespace SeatBookingApi.Domain
{
    public class Canvas:BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}
