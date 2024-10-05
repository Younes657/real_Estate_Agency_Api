using AgenceImmobiliareApi.Models;
using AgenceImmobiliareApi.Repository.IRepository;
using AgenceImmobiliareApi.Utility;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace AgenceImmobiliareApi.Controllers
{
    [Route("api/WebInfo")]
    [ApiController]
    public class WebSiteSettingsController : ControllerBase
    {
        private readonly IUnitOfWork _UnitOfWork;
        private ApiResponse _response;
        private readonly IMapper _Mapper;
        public WebSiteSettingsController(IUnitOfWork unitofWork, IMapper mapper)
        {
            _UnitOfWork = unitofWork;
            _Mapper = mapper;
            _response = new ApiResponse();
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> GetAll()
        {
            try
            {
                IEnumerable<WebSiteInfo> result =  _UnitOfWork.AppDbContext().WebSiteInfos;
                _response.Result = result.FirstOrDefault() ?? new WebSiteInfo();
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
        [HttpPut("{id:int}")]
        [Authorize(Roles = SD.Role_Admin)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse>> UpdateWebSiteInfo(WebSiteInfo webSiteInfo, int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (id == 0 || id != webSiteInfo.Id)
                    {
                        _response.IsSuccess = false;
                        _response.Errors.Add("L'identifiant n'est pas valide !!");
                        _response.StatusCode = HttpStatusCode.BadRequest;
                        return BadRequest(_response);
                    }
                    var infoDb = await  _UnitOfWork.AppDbContext().WebSiteInfos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                    if (infoDb == null)
                    {
                        _response.IsSuccess = false;
                        _response.Errors.Add("les information est introuvable !!");
                        _response.StatusCode = HttpStatusCode.NotFound;
                        return NotFound(_response);
                    }
                    infoDb = webSiteInfo;

                    _UnitOfWork.AppDbContext().WebSiteInfos.Update(infoDb);
                    await _UnitOfWork.Save();
                    _response.IsSuccess = true;
                    _response.StatusCode = HttpStatusCode.OK;
                    _response.Result = infoDb;
                    return Ok(_response);
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.Errors = ModelState.SelectMany(x => x.Value.Errors.Select(p => p.ErrorMessage)).ToList();
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Errors.Add(ex.Message);
                _response.StatusCode = HttpStatusCode.BadRequest;
            }
            return BadRequest(_response);
        }
    }
}
