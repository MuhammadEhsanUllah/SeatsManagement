namespace SeatBookingApi.DTOs
{
    public class GetSection_DTO
    {
        public int Id { get; set; }
        public int SectionNumber { get; set; }
        public int RowsCount { get; set; }
        public int ColumnsCount { get; set; }
        public List<GetSeat_DTO> Seats { get; set; }
    }
}
