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
    public class SeatController : ControllerBase
    {
        private readonly IClientSeatService _clientSeatService;
        public SeatController(IClientSeatService clientSeatService)
        {
            _clientSeatService = clientSeatService;
        }
        /// <summary>
        /// Return a list of reserved seats for client
        /// </summary>
        /// <returns>A list of reserved seats</returns>
        [HttpGet("venues")]
        public async Task<ResponseModel> GetReservedSeats(int clientId)
        {
            var response = new ResponseModel();
            response = await _clientSeatService.GetReservedSeats(clientId);

            return response;
        }
        /// <summary>
        /// toogle reserve seat
        /// </summary>
        /// <returns>toogle reserve seat</returns>
        [HttpPut("reserve")]
        public async Task<ResponseModel> UpdateReserveSeat(UpdateReserveSeat_DTO model)
        {
            var response = new ResponseModel();
            response = await _clientSeatService.UpdateReserveSeat(model);

            return response;
        }
    }
}
