using SeatBookingApi.DTOs;
using SeatBookingApi.ResponseModels;

namespace SeatBookingApi.Interfaces
{
    public interface IClientSeatService
    {
        Task<ResponseModel> GetReservedSeats(int ClientId);
        Task<ResponseModel> UpdateReservedSeats(UpdateReserveSeat_DTO model);
    }
}
