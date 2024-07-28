using AgenceImmobiliareApi.Models;
using AgenceImmobiliareApi.Repository.IRepository;
using AgenceImmobiliareApi.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AgenceImmobiliareApi.Controllers
{
    [Route("api/Image")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private IUnitOfWork _UnitOfWork;
        private ApiResponse _response;
        private readonly IWebHostEnvironment _WebHostEnvironment;

        public ImageController(IUnitOfWork unitOfWork , IWebHostEnvironment webHostEnvironment)
        {
            _UnitOfWork = unitOfWork;
            _WebHostEnvironment = webHostEnvironment;
            _response  = new ApiResponse();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = SD.Role_Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> DeleteImage ( int id, int realEstateId)
        {
            try
            {
                if (id == 0)
                {
                    return NoContent();
                }
                Image imagedb = await _UnitOfWork.ImageRepo.Get(x => x.Id == id);
                if (imagedb == null)
                {
                    return NoContent();
                }
                bool result = _UnitOfWork.ImageRepo.DeleteImage(_WebHostEnvironment, realEstateId, imagedb.ImageLink);
                if (result)
                {
                    _UnitOfWork.ImageRepo.Remove(imagedb);
                    var realEstateDb = await _UnitOfWork.RealEstateRepo.Get(x => x.Id == realEstateId);
                    if (realEstateDb != null) { 
                        realEstateDb.NbImage -= 1;
                        _UnitOfWork.RealEstateRepo.Update(realEstateDb);
                    }
                    await _UnitOfWork.Save();
                    _response.Result = "image deleted successfuly";
                    _response.StatusCode = System.Net.HttpStatusCode.OK;
                    _response.IsSuccess = true;
                    return Ok(_response);
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Errors.Add(ex.Message);
                _response.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
            return NoContent();
        }
    }
}
