namespace SeatBookingApi.DTOs
{
    public class UpdateReserveSeat_DTO
    {
        public int ClientId { get; set; }  
        public List<ReserveSeat> Seats { get; set; }
        public class ReserveSeat
        {
            public int SeatId { get; set; }
            public bool IsReserved { get; set; }
        }
    }
}
