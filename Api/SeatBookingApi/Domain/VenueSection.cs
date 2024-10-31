using System.ComponentModel.DataAnnotations;

namespace SeatBookingApi.Domain
{
    public class VenueSection:BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public int VenueId { get; set; }
        public int SectionId { get; set; }
        public int X {  get; set; }
        public int Y { get; set; }
        // Navigation properties
        public Venue Venue { get; set; }
        public Section Section { get; set; }
    }
}
