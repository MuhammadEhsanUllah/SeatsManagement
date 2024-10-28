using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SeatBookingApi.DTOs;
using SeatBookingApi.Interfaces;
using SeatBookingApi.ResponseModels;
using SeatBookingApi.Services;

namespace SeatBookingApi.Controllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class VenueController : ControllerBase
    {
        private readonly IVenueService _venueService;
        public VenueController(IVenueService venueService)
        {
            _venueService = venueService;
        }
        /// <summary>
        /// Return a list of all Venues for admin
        /// </summary>
        /// <returns>A list of Venues</returns>
        [HttpGet("venues")]
        public async Task<ResponseModel> GetVenues()
        {
            var response = new ResponseModel();
            response = await _venueService.GetVenuesList();

            return response;
        }
        /// <summary>
        /// Add a venue
        /// </summary>
        /// <returns>Add venue</returns>
        [HttpPost("venue")]
        public async Task<ResponseModel> AddVenue(AddVenue_DTO model)
        {
            var response = new ResponseModel();
            response = await _venueService.AddVenue(model);

            return response;
        }
        /// <summary>
        /// delete multiple venues with a list of ids
        /// </summary>
        /// <returns>Delete venues</returns>
        [HttpDelete("venue")]
        public async Task<ResponseModel> DeleteVenues(int[] idsToDelete)
        {
            var response = new ResponseModel();
            response = await _venueService.DeleteVenues(idsToDelete);

            return response;
        }
        /// <summary>
        /// update venue sections
        /// </summary>
        /// <returns>update venue</returns>
        [HttpPut("venue")]
        public async Task<ResponseModel> UpdateVenue(UpdateVenue_DTO model)
        {
            var response = new ResponseModel();
            response = await _venueService.UpdateVenue(model);

            return response;
        }
    }
}
