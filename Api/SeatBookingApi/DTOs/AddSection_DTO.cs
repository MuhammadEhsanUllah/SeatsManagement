namespace SeatBookingApi.DTOs
{
    public class AddSection_DTO
    {
        public string Name { get; set; }
        public int RowsCount { get; set; }
        public int ColumnsCount { get; set; }
        public List<AddSeat_DTO> Seats { get; set; }
    }
}
