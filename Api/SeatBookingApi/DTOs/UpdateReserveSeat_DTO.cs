namespace SeatBookingApi.DTOs
{
    public class UpdateReserveSeat_DTO
    {
        public int SeatId { get; set; }
        public int ClientId { get; set; }
        public bool IsReserved { get; set; }

    }
}
