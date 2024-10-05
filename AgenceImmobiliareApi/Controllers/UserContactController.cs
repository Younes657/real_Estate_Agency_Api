using AgenceImmobiliareApi.Models;
using AgenceImmobiliareApi.Models.DTOs;
using AgenceImmobiliareApi.Repository.IRepository;
using AgenceImmobiliareApi.Services;
using AgenceImmobiliareApi.Utility;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AgenceImmobiliareApi.Controllers
{
    [Route("api/UserContact")]
    [ApiController]
    public class UserContactController : ControllerBase
    {
        private readonly IUnitOfWork _UnitOfWork;
        private ApiResponse _response;
        private readonly IMapper _Mapper;
        private readonly IEmailService _EmailService;
        public UserContactController(IUnitOfWork unitofWork, IMapper mapper , IEmailService emailService)
        {
            _UnitOfWork = unitofWork;
            _Mapper = mapper;
            _response = new ApiResponse();
            _EmailService = emailService;
        }

        [HttpGet]
        [Authorize(Roles =SD.Role_Admin)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> GetAll()
        {
            try
            {
                IEnumerable<UserContact> result = await _UnitOfWork.UserContactRepo.GetAll();
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
        [HttpGet("{id:int}", Name = "GetUserContact")]
        [Authorize(Roles =SD.Role_Admin)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse>> GetOne(int id)
        {
            if (id == 0)
            {
                _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.Errors.Add("les Données fournies est invalid");
                return BadRequest(_response);
            }
            UserContact? userContact = null;
            try
            {
                userContact = await _UnitOfWork.UserContactRepo.Get(x => x.Id == id);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Errors.Add(ex.Message);
                _response.Errors.Add("Error occured during the data fetching ");
                _response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
            if (userContact == null)
            {
                _response.StatusCode = System.Net.HttpStatusCode.NotFound;
                _response.Errors.Add("Il n'y a aucune Contact correspondant aux données fournies");
                _response.IsSuccess = false;
                return NotFound(_response);
            }
            _response.Result = userContact;
            _response.StatusCode = System.Net.HttpStatusCode.OK;
            _response.IsSuccess = true;
            return Ok(_response);
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse>> CreateContact([FromForm] UserContactCreateDto userDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    UserContact userContact = _Mapper.Map<UserContact>(userDto);
                    userContact.Seen = false;
                    userContact.CreatedDate = DateTime.Now;
                    await _UnitOfWork.UserContactRepo.Add(userContact);
                    await _UnitOfWork.Save();
                    EmailRequest emailRequest = new EmailRequest()
                    {
                        ToEmail = "younesbc2123@gmail.com",
                        Subject = "message de site Immobilier Nari",
                        Body = $"<p>Vous avez un Message envoyé par {userContact.Name}</p>" +
                        $"<p>Email : {userContact.Email}</p>" + 
                        $"<p>Numéro de Téléphone : {userContact.PhoneNumber}</p>"+
                        $"<p>Théme : {userContact.Sujet}</p>" + 
                        $"<p>Consulter le Site Web pour plus de Détails </p>"
                    };
                    await _EmailService.SendEmailAsync(emailRequest.ToEmail, emailRequest.Subject, emailRequest.Body);

                    _response.IsSuccess = true;
                    _response.StatusCode = HttpStatusCode.Created;
                    _response.Result = userContact;
                    return CreatedAtRoute("GetUserContact", new { id = userContact.Id }, _response);
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

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> DeleteContact(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.IsSuccess = false;
                    _response.Errors.Add("L'identifiant n'est pas valide !!");
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }
                UserContact userContact = await _UnitOfWork.UserContactRepo.Get(x => x.Id == id);
                if (userContact == null)
                {
                    _response.IsSuccess = false;
                    _response.Errors.Add("Contact est introuvable !!");
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                //Thread.Sleep(2000);
                //you should add a warning in the front because deleting a category gonna cause deleting all realestates of it
                _UnitOfWork.UserContactRepo.Remove(userContact);
                await _UnitOfWork.Save();
                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.NoContent;
                return NoContent();
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Errors.Add(ex.Message);
                _response.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }
        [HttpPut("{id:int}")]
        [Authorize(Roles = SD.Role_Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse>> UpdateBlog(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.IsSuccess = false;
                    _response.Errors.Add("L'identifiant n'est pas valide !!");
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }
                var userContact = await _UnitOfWork.UserContactRepo.Get(x => x.Id == id, tracked:true);
                if (userContact == null)
                {
                    _response.IsSuccess = false;
                    _response.Errors.Add("Contact est introuvable !!");
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }
                userContact.Seen = true;
                await _UnitOfWork.Save();
                return NoContent();
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
