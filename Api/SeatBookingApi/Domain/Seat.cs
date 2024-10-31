using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeatBookingApi.Domain
{
    public class Seat:BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public float Radius { get; set; }
        public string Price { get; set; }
        public string Color { get; set; }
        public bool IsReserved { get; set; }
        public int? ClientId { get; set; }
        public int SectionId { get; set; }

        [ForeignKey("SectionId")]
        public virtual Section Section { get; set; }

    }
}
