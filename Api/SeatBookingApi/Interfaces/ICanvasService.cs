using SeatBookingApi.DTOs;
using SeatBookingApi.ResponseModels;

namespace SeatBookingApi.Interfaces
{
    public interface ICanvasService
    {
        Task<ResponseModel> GetCanvasesList();
        Task<ResponseModel> AddCanvas(AddCanvas_DTO model);
        Task<ResponseModel> DeleteCanvases(int[] idsToDelete);
    }
}
