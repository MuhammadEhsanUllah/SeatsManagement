namespace SeatBookingApi.DTOs
{
    public class AddVenue_DTO
    {
        public string Name { get; set;}
        public List<AddVenueSection_DTO> Sections { get; set;}
    }
}
