namespace SeatBookingApi.DTOs
{
    public class GetVenue_DTO
    {
        public int Id { get; set; }
        public string Name { get; set;}
        public List<GetSection_DTO> Sections { get; set;}
    }
}
