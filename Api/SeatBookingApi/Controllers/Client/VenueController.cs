using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SeatBookingApi.DTOs;
using SeatBookingApi.Interfaces;
using SeatBookingApi.ResponseModels;
using SeatBookingApi.Services;

namespace SeatBookingApi.Controllers.Client
{
    [Route("api/client/[controller]")]
    [ApiController]
    public class VenueController : ControllerBase
    {
        private readonly IClientVenueService _clientVenueService;
        public VenueController(IClientVenueService clientVenueService)
        {
            _clientVenueService = clientVenueService;
        }
        /// <summary>
        /// Return a list of all Venues for client
        /// </summary>
        /// <returns>A list of Venues</returns>
        [HttpGet("venues")]
        public async Task<ResponseModel> GetVenues()
        {
            var response = new ResponseModel();
            response = await _clientVenueService.GetVenuesList();

            return response;
        }
    }
}
