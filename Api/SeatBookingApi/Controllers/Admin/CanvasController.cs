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
    public class CanvasController : ControllerBase
    {
        private readonly ICanvasService _canvasService;
        public CanvasController(ICanvasService canvasService)
        {
            _canvasService = canvasService;
        }
        /// <summary>
        /// Return a list of all Canvases for admin
        /// </summary>
        /// <returns>A list of Canvases</returns>
        [HttpGet("canvases")]
        public async Task<ResponseModel> GetCanvases()
        {
            var response = new ResponseModel();
            response = await _canvasService.GetCanvasesList();

            return response;
        }
        /// <summary>
        /// Add a canvas
        /// </summary>
        /// <returns>Add canvas</returns>
        [HttpPost("canvas")]
        public async Task<ResponseModel> AddCanvas(AddCanvas_DTO model)
        {
            var response = new ResponseModel();
            response = await _canvasService.AddCanvas(model);

            return response;
        }
        /// <summary>
        /// delete multiple canvases with a list of ids
        /// </summary>
        /// <returns>Delete canvases</returns>
        [HttpDelete("canvas")]
        public async Task<ResponseModel> DeleteCanvases(int[] idsToDelete)
        {
            var response = new ResponseModel();
            response = await _canvasService.DeleteCanvases(idsToDelete);

            return response;
        }
    }
}
