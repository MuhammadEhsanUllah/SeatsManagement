using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SeatBookingApi.DTOs;
using SeatBookingApi.Interfaces;
using SeatBookingApi.ResponseModels;

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
