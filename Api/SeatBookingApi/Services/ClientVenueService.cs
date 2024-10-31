using Microsoft.EntityFrameworkCore;
using SeatBookingApi.Domain;
using SeatBookingApi.DTOs;
using SeatBookingApi.Interfaces;
using SeatBookingApi.ResponseModels;
using System.Security.Cryptography.X509Certificates;

namespace SeatBookingApi.Services
{
    public class ClientVenueService : IClientVenueService
    {
        private readonly ApplicationDbContext _context;
        public ClientVenueService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<ResponseModel> GetVenuesList()
        {
            try
            {
                var venuees = await _context.Venues
                .Where(v => v.IsDeleted != true)
                .Select(v => new GetVenue_DTO
                {
                    Id = v.Id,
                    Name = v.Name,
                    Sections = v.VenueSections
                        .Where(vs => vs.Section.IsDeleted != true && vs.IsDeleted != true)
                        .Select(vs => new GetSection_DTO
                        {
                            Id = vs.Section.Id,
                            //SectionNumber = vs.Section.SectionNumber,
                            Name = vs.Section.Name,
                            RowsCount = vs.Section.RowsCount,
                            ColumnsCount = vs.Section.ColumnsCount,
                            X = vs.X,
                            Y = vs.Y,
                            Seats = vs.Section.Seats
                                .Where(s => s.IsDeleted != true)
                                .Select(s => new GetSeat_DTO
                                {
                                    Id = s.Id,
                                    IsReserved = s.IsReserved,
                                    X = s.X,
                                    Y = s.Y,
                                    Radius = s.Radius,
                                    Price = s.Price,
                                    Color = s.Color
                                })
                                .ToList()
                        })
                        .ToList()
                })
                .ToListAsync();

                return ResponseModel.SuccessResponse(venuees);
            }
            catch (Exception ex)
            {
                return ResponseModel.ErrorResponse(ex.Message);
            }
        }

    }
}
