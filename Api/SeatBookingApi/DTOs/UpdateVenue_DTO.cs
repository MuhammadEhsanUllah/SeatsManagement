namespace SeatBookingApi.DTOs
{
    public class UpdateVenue_DTO
    {
        public int Id { get; set; }
        public List<int> SectionIds { get; set; }
    }
}
