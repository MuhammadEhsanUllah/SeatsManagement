namespace SeatBookingApi.DTOs
{
    public class UpdateVenue_DTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<AddVenueSection_DTO> Sections { get; set; }
    }
}
