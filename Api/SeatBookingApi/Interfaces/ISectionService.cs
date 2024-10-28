﻿using SeatBookingApi.DTOs;
using SeatBookingApi.ResponseModels;

namespace SeatBookingApi.Interfaces
{
    public interface ISectionService
    {
        Task<ResponseModel> GetSectionsList();
        Task<ResponseModel> AddSection(AddSection_DTO model);
        Task<ResponseModel> DeleteSections(int[] idsToDelete);
    }
}