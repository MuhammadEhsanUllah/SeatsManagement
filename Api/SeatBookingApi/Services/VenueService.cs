using Microsoft.EntityFrameworkCore;
using SeatBookingApi.Domain;
using SeatBookingApi.DTOs;
using SeatBookingApi.Interfaces;
using SeatBookingApi.ResponseModels;
using System.Security.Cryptography.X509Certificates;

namespace SeatBookingApi.Services
{
    public class VenueService : IVenueService
    {
        private readonly ApplicationDbContext _context;
        public VenueService(ApplicationDbContext context)
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
        public async Task<ResponseModel> AddVenue(AddVenue_DTO model)
        {
            try
            {
                var venue = new Venue()
                {
                    Name = model.Name,
                    CreatedDate = DateTime.Now,
                    IsDeleted = false
                };
                await _context.Venues.AddAsync(venue);
                await _context.SaveChangesAsync();

                foreach (var sectionId in model.SectionIds)
                {
                    var venueSection = new VenueSection
                    {
                        VenueId = venue.Id,
                        SectionId = sectionId,
                        CreatedDate = DateTime.Now,
                        IsDeleted = false
                    };
                    await _context.VenueSections.AddAsync(venueSection);
                }

                await _context.SaveChangesAsync();

                return ResponseModel.SuccessResponse(venue, "Venue added successfully");
            }
            catch (Exception ex)
            {
                return ResponseModel.ErrorResponse(ex.Message);
            }
        }
        public async Task<ResponseModel> DeleteVenues(int[] idsToDelete)
        {
            try
            {
                var venuees = await _context.Venues
                    .Where(x => x.IsDeleted != true && idsToDelete.Contains(x.Id))
                    .ToListAsync();
                if (venuees.Any())
                {
                    foreach (var venue in venuees)
                    {
                        venue.IsDeleted = true;
                    }
                }
                await _context.SaveChangesAsync();

                return ResponseModel.SuccessResponse("Venuees deleted successfully");
            }
            catch (Exception ex)
            {
                return ResponseModel.ErrorResponse(ex.Message);
            }
        }
        public async Task<ResponseModel> UpdateVenue(UpdateVenue_DTO model)
        {
            try
            {
                var venue = await _context.Venues
                    .Where(x => x.IsDeleted != true)
                    .Include(v => v.VenueSections)
                    .FirstOrDefaultAsync(v => v.Id == model.Id);

                if (venue == null)
                {
                    return ResponseModel.ErrorResponse("Not Found.");
                }
                venue.Name = model.Name;
                var existingSectionIds = venue.VenueSections
                    .Where(vs => vs.IsDeleted != true)
                    .Select(vs => vs.SectionId)
                    .ToList();

                var sectionsToAdd = model.SectionIds.Except(existingSectionIds).ToList();
                var sectionsToRemove = existingSectionIds.Except(model.SectionIds).ToList();

                foreach (var sectionId in sectionsToAdd)
                {
                    venue.VenueSections.Add(new VenueSection
                    {
                        VenueId = model.Id,
                        SectionId = sectionId,
                        CreatedDate = DateTime.Now,
                        IsDeleted = false
                    });
                }

                foreach (var section in venue.VenueSections.Where(vs => sectionsToRemove.Contains(vs.SectionId)))
                {
                    section.IsDeleted = true;
                    section.DateUpdated = DateTime.Now;
                }

                await _context.SaveChangesAsync();
                return ResponseModel.SuccessResponse("Venue updated successfully");
            }
            catch (Exception ex)
            {
                return ResponseModel.ErrorResponse(ex.Message);
            }
        }

    }
}
