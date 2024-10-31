namespace SeatBookingApi.DTOs
{
    public class UpdateSectionPosition_DTO
    {
        public int VenueId { get; set; }
        public int SectionId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}
