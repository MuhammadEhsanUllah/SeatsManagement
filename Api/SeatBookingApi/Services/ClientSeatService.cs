using Microsoft.EntityFrameworkCore;
using SeatBookingApi.Domain;
using SeatBookingApi.DTOs;
using SeatBookingApi.Interfaces;
using SeatBookingApi.ResponseModels;
using System.Security.Cryptography.X509Certificates;

namespace SeatBookingApi.Services
{
    public class ClientSeatService : IClientSeatService
    {
        private readonly ApplicationDbContext _context;
        public ClientSeatService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseModel> GetReservedSeats(int ClientId)
        {
            try
            {
                var seats = await _context.Seats
                    .Where(x => x.ClientId == ClientId && x.IsReserved == true && x.IsDeleted != true)
                    .Select(x=>new GetClientSeat_DTO
                    {
                        Id = x.Id,
                        Color = x.Color,
                        Price = x.Price,
                        Radius = x.Radius,
                        X = x.X,
                        Y = x.Y
                    })
                    .ToListAsync();

                return ResponseModel.SuccessResponse(seats);
            }
            catch (Exception ex)
            {
                return ResponseModel.ErrorResponse(ex.Message);
            }
        }

        public async Task<ResponseModel> UpdateReserveSeat(UpdateReserveSeat_DTO model)
        {
            try
            {
                var seat = await _context.Seats
                    .Where(x => x.Id == model.SeatId && x.IsDeleted != true).FirstOrDefaultAsync();
                if (seat == null)
                    return ResponseModel.ErrorResponse("Not Found");
                if (seat.ClientId != null && seat.ClientId != model.ClientId)
                    return ResponseModel.ErrorResponse("Seat already reserved by another person.");

                seat.ClientId = model.ClientId;
                seat.IsReserved = model.IsReserved;
                seat.DateUpdated = DateTime.Now;

                await _context.SaveChangesAsync();
                return ResponseModel.SuccessResponse("Seat reserved successfully");
            }
            catch (Exception ex)
            {
                return ResponseModel.ErrorResponse(ex.Message);
            }
        }

    }
}
