namespace SeatBookingApi.DTOs
{
    public class GetVenue_DTO
    {
        public string Name { get; set;}
        public List<GetSection_DTO> Sections { get; set;}
    }
}
