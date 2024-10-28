using System.ComponentModel.DataAnnotations;

namespace SeatBookingApi.Domain
{
    public class Venue:BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        // Navigation property
        public ICollection<VenueSection> VenueSections { get; set; }
    }
}
