using SeatBookingApi.DTOs;
using SeatBookingApi.ResponseModels;

namespace SeatBookingApi.Interfaces
{
    public interface IClientVenueService
    {
        Task<ResponseModel> GetVenuesList();
    }
}
