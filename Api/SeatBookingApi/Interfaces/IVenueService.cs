using SeatBookingApi.DTOs;
using SeatBookingApi.ResponseModels;

namespace SeatBookingApi.Interfaces
{
    public interface IVenueService
    {
        Task<ResponseModel> GetVenuesList();
        Task<ResponseModel> AddVenue(AddVenue_DTO model);
        Task<ResponseModel> DeleteVenues(int[] idsToDelete);
        Task<ResponseModel> UpdateVenue(UpdateVenue_DTO model);
        Task<ResponseModel> UpdateSectionPosition(UpdateSectionPosition_DTO model);
    }
}
