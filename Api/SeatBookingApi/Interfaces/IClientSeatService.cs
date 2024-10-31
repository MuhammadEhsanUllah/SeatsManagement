using SeatBookingApi.DTOs;
using SeatBookingApi.ResponseModels;

namespace SeatBookingApi.Interfaces
{
    public interface IClientSeatService
    {
        Task<ResponseModel> UpdateReserveSeat(UpdateReserveSeat_DTO model);
    }
}
