namespace SeatBookingApi.DTOs
{
    public class UpdateSection_DTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int RowsCount { get; set; }
        public int ColumnsCount { get; set; }
        public List<UpdateSeat_DTO> Seats { get; set; }
    }
}
