namespace SeatBookingApi.Domain
{
    public class BaseEntity
    {
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? DateUpdated { get; set; }
    }
}
