namespace SeatBookingApi.DTOs
{
    public class GetSeat_DTO:AddSeat_DTO
    {
        public int Id { get; set; }
        public bool IsReserved { get; set; }
    }
}
