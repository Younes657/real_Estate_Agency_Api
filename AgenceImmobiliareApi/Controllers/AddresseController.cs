using AgenceImmobiliareApi.Models;
using AgenceImmobiliareApi.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AgenceImmobiliareApi.Controllers
{
    [Route("api/Addresse")]
    [ApiController]
    public class AddresseController : ControllerBase
    {
        private IUnitOfWork _UnitOfWork;
        private ApiResponse _response;

        public AddresseController(IUnitOfWork unitOfWork)
        {
            _UnitOfWork = unitOfWork;
            _response = new ApiResponse();
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> GetAll()
        {
            try
            {
                IEnumerable<Addresse> result = await _UnitOfWork.AddresseRepo.GetAll();
                _response.Result = result ?? [];
                _response.StatusCode = System.Net.HttpStatusCode.OK;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Errors.Add(ex.Message);
                _response.Errors.Add("Error occured during the data fetching ");
                _response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }
    }
}
