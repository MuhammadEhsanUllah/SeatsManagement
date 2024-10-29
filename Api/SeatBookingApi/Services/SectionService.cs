using Microsoft.EntityFrameworkCore;
using SeatBookingApi.Domain;
using SeatBookingApi.DTOs;
using SeatBookingApi.Interfaces;
using SeatBookingApi.ResponseModels;
using System.Security.Cryptography.X509Certificates;

namespace SeatBookingApi.Services
{
    public class SectionService : ISectionService
    {
        private readonly ApplicationDbContext _context;
        public SectionService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<ResponseModel> GetSectionsList()
        {
            var sections = await _context.Sections
                .Where(x => x.IsDeleted != true).Select(x => new GetSection_DTO()
                {
                    Id = x.Id,
                    RowsCount = x.RowsCount,
                    ColumnsCount = x.ColumnsCount,
                    //SectionNumber = x.SectionNumber,
                    Name = x.Name,
                    Seats = _context.Seats.Where(y => y.SectionId == x.Id).Select(s => new GetSeat_DTO()
                    {
                        Id = s.Id,
                        Color = s.Color,
                        IsReserved = s.IsReserved,
                        Price = s.Price,
                        Radius = s.Radius,
                        X = s.X,
                        Y = s.Y,
                    }).ToList()
                })
                .ToListAsync();
            return ResponseModel.SuccessResponse(sections);
        }
        public async Task<ResponseModel> AddSection(AddSection_DTO model)
        {
            //var prevSection = await _context.Sections
            // .Where(x => x.IsDeleted != true)
            // .OrderByDescending(x => x.SectionNumber)
            // .FirstOrDefaultAsync();

            //int prevSectionNumber = prevSection?.SectionNumber ?? 0;

            var section = new Section()
            {
                RowsCount = model.RowsCount,
                ColumnsCount = model.ColumnsCount,
                Name = model.Name,
                //SectionNumber = prevSectionNumber + 1,
                CreatedDate = DateTime.Now,
                IsDeleted = false
            };
            await _context.Sections.AddAsync(section);
            await _context.SaveChangesAsync();
            var seats = new List<Seat>();
            foreach (var s in model.Seats)
            {
                var seat = new Seat()
                {
                    X = s.X,
                    Y = s.Y,
                    Price = s.Price,
                    Color = s.Color,
                    Radius = s.Radius,
                    SectionId = section.Id,
                    IsReserved = false,
                    CreatedDate = DateTime.Now,
                    IsDeleted = false,
                };
                seats.Add(seat);
            }
            await _context.AddRangeAsync(seats);
            await _context.SaveChangesAsync();


            return ResponseModel.SuccessResponse(model, "Section added successfully");
        }
        public async Task<ResponseModel> DeleteSections(int[] idsToDelete)
        {
            try
            {
                var sections = await _context.Sections
                    .Where(x => x.IsDeleted != true && idsToDelete.Contains(x.Id))
                    .ToListAsync();
                if (sections.Any())
                {
                    foreach (var section in sections)
                    {
                        section.IsDeleted = true;
                        section.DateUpdated = DateTime.Now;
                    }
                }
                await _context.SaveChangesAsync();

                return ResponseModel.SuccessResponse("Sections deleted successfully");
            }
            catch (Exception ex)
            {
                return ResponseModel.ErrorResponse(ex.Message);
            }
        }
        public async Task<ResponseModel> UpdateSection(UpdateSection_DTO model)
        {
            try
            {
                // Fetch section along with seats from database
                var section = await _context.Sections
                    .Where(s => s.IsDeleted != true)
                    .Include(s => s.Seats)
                    .FirstOrDefaultAsync(s => s.Id == model.Id);

                if (section == null)
                {
                    return ResponseModel.ErrorResponse("Section not found.");
                }

                // Update section details
                section.Name = model.Name;
                section.RowsCount = model.RowsCount;
                section.ColumnsCount = model.ColumnsCount;
                section.DateUpdated = DateTime.Now;

                var existingSeatIds = section.Seats
                    .Where(seat => seat.IsDeleted != true)
                    .Select(seat => seat.Id)
                    .ToList();

                var modelSeatIds = model.Seats.Select(seat => seat.Id).ToList();

                var seatsToAdd = model.Seats.Where(seat => seat.Id == 0).ToList(); // New seats
                var seatsToUpdate = model.Seats.Where(seat => existingSeatIds.Contains(seat.Id)).ToList(); // Existing seats to update
                var seatsToDelete = section.Seats
                    .Where(seat => !modelSeatIds.Contains(seat.Id) && seat.IsDeleted != true)
                    .ToList();

                // Add new seats
                foreach (var seat in seatsToAdd)
                {
                    section.Seats.Add(new Seat
                    {
                        X = seat.X,
                        Y = seat.Y,
                        Radius = seat.Radius,
                        Price = seat.Price,
                        Color = seat.Color,
                        CreatedDate = DateTime.Now,
                        IsDeleted = false
                    });
                }

                // Update existing seats
                foreach (var seat in seatsToUpdate)
                {
                    var existingSeat = section.Seats.FirstOrDefault(s => s.Id == seat.Id);
                    if (existingSeat != null)
                    {
                        existingSeat.X = seat.X;
                        existingSeat.Y = seat.Y;
                        existingSeat.Radius = seat.Radius;
                        existingSeat.Price = seat.Price;
                        existingSeat.Color = seat.Color;
                        existingSeat.DateUpdated = DateTime.Now;
                    }
                }

                foreach (var seat in seatsToDelete)
                {
                    seat.IsDeleted = true;
                    seat.DateUpdated = DateTime.Now;
                }

                await _context.SaveChangesAsync();
                return ResponseModel.SuccessResponse("Section updated successfully");
            }
            catch (Exception ex)
            {
                return ResponseModel.ErrorResponse(ex.Message);
            }
        }

    }
}
