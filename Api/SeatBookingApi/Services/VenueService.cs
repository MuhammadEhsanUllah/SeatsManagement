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
        public async Task<ResponseModel> AddVenue(AddVenue_DTO model)
        {
            try
            {
                var existingVenue = await _context.Venues
                .Where(x => x.Name == model.Name && x.IsDeleted != true).FirstOrDefaultAsync();
                if (existingVenue != null)
                    return ResponseModel.ErrorResponse("Venue already exists with this name");

                var venue = new Venue()
                {
                    Name = model.Name,
                    CreatedDate = DateTime.Now,
                    IsDeleted = false
                };
                await _context.Venues.AddAsync(venue);
                await _context.SaveChangesAsync();

                foreach (var section in model.Sections)
                {
                    var venueSection = new VenueSection
                    {
                        VenueId = venue.Id,
                        SectionId = section.SectionId,
                        X = section.X,
                        Y = section.Y,
                        CreatedDate = DateTime.Now,
                        IsDeleted = false
                    };
                    await _context.VenueSections.AddAsync(venueSection);
                }

                await _context.SaveChangesAsync();

                return ResponseModel.SuccessResponse("Venue added successfully");
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
                        venue.DateUpdated = DateTime.Now;
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
                    .Where(x => !x.IsDeleted && x.Id == model.Id)
                    .Include(v => v.VenueSections.Where(vs => vs.IsDeleted != true))
                    .FirstOrDefaultAsync();

                if (venue == null)
                {
                    return ResponseModel.ErrorResponse("Venue not found.");
                }

                // Update Venue properties
                venue.Name = model.Name;

                var existingSectionIds = venue.VenueSections.Select(vs => vs.SectionId).ToList();
                var newSectionIds = model.Sections.Select(s => s.SectionId).ToList();

                var sectionsToAdd = newSectionIds.Except(existingSectionIds).ToList();
                var sectionsToRemove = existingSectionIds.Except(newSectionIds).ToList();
                var sectionsToUpdate = existingSectionIds.Intersect(newSectionIds).ToList();

                // Add new sections
                foreach (var sectionId in sectionsToAdd)
                {
                    var newSection = model.Sections.FirstOrDefault(x => x.SectionId == sectionId);
                    if (newSection == null) continue;

                    venue.VenueSections.Add(new VenueSection
                    {
                        VenueId = model.Id,
                        SectionId = sectionId,
                        X = newSection.X,
                        Y = newSection.Y,
                        CreatedDate = DateTime.UtcNow,
                        IsDeleted = false
                    });
                }

                // Update existing sections
                foreach (var sectionId in sectionsToUpdate)
                {
                    var section = model.Sections.FirstOrDefault(x => x.SectionId == sectionId);
                    var dbSection = venue.VenueSections.FirstOrDefault(vs => vs.SectionId == sectionId);

                    if (section != null && dbSection != null)
                    {
                        dbSection.X = section.X;
                        dbSection.Y = section.Y;
                        dbSection.DateUpdated = DateTime.Now;
                    }
                }

                foreach (var section in venue.VenueSections.Where(vs => sectionsToRemove.Contains(vs.SectionId)))
                {
                    section.IsDeleted = true;
                    section.DateUpdated = DateTime.Now;
                }

                await _context.SaveChangesAsync();
                return ResponseModel.SuccessResponse("Venue updated successfully.");
            }
            catch (Exception ex)
            {
                return ResponseModel.ErrorResponse($"An error occurred while updating the venue: {ex.Message}");
            }
        }
        public async Task<ResponseModel> UpdateSectionPosition(UpdateSectionPosition_DTO model)
        {
            try
            {
                var venueSection = await _context.VenueSections
                    .Where(x => x.VenueId == model.VenueId && x.SectionId == model.SectionId && x.IsDeleted != true).FirstOrDefaultAsync();
                if (venueSection != null)
                {
                    venueSection.X = model.X;
                    venueSection.Y = model.Y;
                    venueSection.DateUpdated = DateTime.Now;
                    await _context.SaveChangesAsync();
                }
                else
                    return ResponseModel.ErrorResponse("Not Found.");

                return ResponseModel.SuccessResponse("Section updated successfully.");
            }
            catch (Exception ex)
            {
                return ResponseModel.ErrorResponse(ex.Message);
            }
        }

    }
}
