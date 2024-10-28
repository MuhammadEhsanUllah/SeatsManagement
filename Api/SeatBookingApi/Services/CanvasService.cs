using Microsoft.EntityFrameworkCore;
using SeatBookingApi.Domain;
using SeatBookingApi.DTOs;
using SeatBookingApi.Interfaces;
using SeatBookingApi.ResponseModels;
using System.Security.Cryptography.X509Certificates;

namespace SeatBookingApi.Services
{
    public class CanvasService : ICanvasService
    {
        private readonly ApplicationDbContext _context;
        public CanvasService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<ResponseModel> GetCanvasesList()
        {
            var canvases = await _context.Canvases
                .Where(x => x.IsDeleted != true)
                .Select(x => new GetCanvas_DTO()
                {
                    Id = x.Id,
                    Height = x.Height,
                    Width = x.Width,
                    X = x.X,
                    Y = x.Y
                }).ToListAsync();

            return ResponseModel.SuccessResponse(canvases);
        }
        public async Task<ResponseModel> AddCanvas(AddCanvas_DTO model)
        {
            try
            {
                var canvas = new Canvas()
                {
                    Width = model.Width,
                    Height = model.Height,
                    X = model.X,
                    Y = model.Y,
                    CreatedDate = DateTime.Now,
                    IsDeleted = false
                };
                await _context.Canvases.AddAsync(canvas);
                await _context.SaveChangesAsync();

                return ResponseModel.SuccessResponse(canvas, "Canvas added successfully");
            }
            catch (Exception ex)
            {
                return ResponseModel.ErrorResponse(ex.Message);
            }
        }
        public async Task<ResponseModel> DeleteCanvases(int[] idsToDelete)
        {
            try
            {
                var canvases = await _context.Canvases
                    .Where(x => x.IsDeleted != true && idsToDelete.Contains(x.Id))
                    .ToListAsync();
                if (canvases.Any())
                {
                    foreach (var canvas in canvases)
                    {
                        canvas.IsDeleted = true;
                    }
                }
                await _context.SaveChangesAsync();

                return ResponseModel.SuccessResponse("Canvases deleted successfully");
            }
            catch (Exception ex)
            {
                return ResponseModel.ErrorResponse(ex.Message);
            }
        }

    }
}
