using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
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
                    Seats = _context.Seats.Where(y => y.SectionId == x.Id && y.IsDeleted != true).Select(s => new GetSeat_DTO()
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
            var existingSection = await _context.Sections
                .Where(x => x.Name == model.Name && x.IsDeleted != true).FirstOrDefaultAsync();
            if (existingSection != null)
                return ResponseModel.ErrorResponse("Section already exists with this name");

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
                var section = await _context.Sections
                    .Where(s => !s.IsDeleted && s.Id == model.Id)
                    .Include(s => s.Seats)
                    .FirstOrDefaultAsync();

                if (section == null)
                {
                    return ResponseModel.ErrorResponse("Section not found.");
                }

                section.Name = model.Name;
                section.DateUpdated = DateTime.Now;

                var modelSeatIds = model.Seats.Select(s => s.Id).ToList();
                foreach (var modelSeat in model.Seats)
                {
                    var seat = section.Seats.FirstOrDefault(seat => seat.Id == modelSeat.Id && !seat.IsDeleted);
                    if (seat != null)
                    {
                        seat.Price = modelSeat.Price;
                        seat.X = modelSeat.X;
                        seat.Y = modelSeat.Y;
                        seat.Radius = modelSeat.Radius;
                        seat.Color = modelSeat.Color;
                        seat.DateUpdated = DateTime.Now;
                    }
                }

                foreach (var existingSeat in section.Seats)
                {
                    if (!modelSeatIds.Contains(existingSeat.Id) && !existingSeat.IsDeleted)
                    {
                        existingSeat.IsDeleted = true;
                        existingSeat.DateUpdated = DateTime.Now;
                    }
                }

                await _context.SaveChangesAsync();
                return ResponseModel.SuccessResponse("Section updated successfully.");
            }
            catch (Exception ex)
            {
                return ResponseModel.ErrorResponse(ex.Message);
            }
        }


        public async Task<ResponseModel> RestoreSectionSeats(int sectionId)
        {
            try
            {
                var seats = await _context.Seats
                    .Where(x => x.IsDeleted == true && x.SectionId == sectionId).ToListAsync();
                foreach (var seat in seats)
                {
                    seat.IsDeleted = false;
                    seat.DateUpdated = DateTime.Now;
                }

                await _context.SaveChangesAsync();

                return ResponseModel.SuccessResponse("Seats restored successfully");
            }
            catch (Exception ex)
            {
                return ResponseModel.ErrorResponse(ex.Message);
            }
        }


    }
}
