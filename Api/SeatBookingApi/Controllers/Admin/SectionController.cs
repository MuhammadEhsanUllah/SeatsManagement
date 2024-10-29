using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SeatBookingApi.Domain;
using SeatBookingApi.DTOs;
using SeatBookingApi.Interfaces;
using SeatBookingApi.ResponseModels;
using SeatBookingApi.Services;

namespace SeatBookingApi.Controllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class SectionController : ControllerBase
    {
        private readonly ISectionService _sectionService;
        public SectionController(ISectionService sectionService)
        {
            _sectionService = sectionService;
        }
        /// <summary>
        /// Return a list of all Sections for admin
        /// </summary>
        /// <returns>A list of Sections</returns>
        [HttpGet("sections")]
        public async Task<ResponseModel> GetSections()
        {
            var response = new ResponseModel();
            response = await _sectionService.GetSectionsList();

            return response;
        }
        /// <summary>
        /// Add a section with seats
        /// </summary>
        /// <returns>Add section</returns>
        [HttpPost("section")]
        public async Task<ResponseModel> AddSection(AddSection_DTO model)
        {
            var response = new ResponseModel();
            response = await _sectionService.AddSection(model);

            return response;
        }
        /// <summary>
        /// delete multiple sections with a list of ids
        /// </summary>
        /// <returns>Delete sections</returns>
        [HttpDelete("section")]
        public async Task<ResponseModel> DeleteSections(int[] idsToDelete)
        {
            var response = new ResponseModel();
            response = await _sectionService.DeleteSections(idsToDelete);

            return response;
        }
        /// <summary>
        /// update section with seats
        /// </summary>
        /// <returns>update section</returns>
        [HttpPut("section")]
        public async Task<ResponseModel> UpdateSection(UpdateSection_DTO model)
        {
            var response = new ResponseModel();
            response = await _sectionService.UpdateSection(model);

            return response;
        }
        /// <summary>
        /// restore seats of specific section
        /// </summary>
        /// <returns>restore seats</returns>
        [HttpPost("{sectionId}/restore")]
        public async Task<ResponseModel> RestoreSectionSeats(int sectionId)
        {
            var response = new ResponseModel();
            response = await _sectionService.RestoreSectionSeats(sectionId);

            return response;
        }
    }
}
