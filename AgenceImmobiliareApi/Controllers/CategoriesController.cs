using AgenceImmobiliareApi.Models;
using AgenceImmobiliareApi.Models.DTOs;
using AgenceImmobiliareApi.Repository;
using AgenceImmobiliareApi.Repository.IRepository;
using AgenceImmobiliareApi.Utility;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AgenceImmobiliareApi.Controllers
{
    [Route("api/Category")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IUnitOfWork _UnitOfWork;
        private ApiResponse _response;
        private readonly IMapper _Mapper;
        public CategoriesController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _UnitOfWork = unitOfWork;
            _response = new ApiResponse();
            _Mapper = mapper;
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> GetAll()
        {
            try
            {
                IEnumerable<Category> result = await _UnitOfWork.CategoryRepo.GetAll();
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
        [HttpGet("{id:int}", Name = "GetCategory")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> GetOne(int id)
        {
            if (id == 0)
            {
                _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.Errors.Add("Id is not recognized !!");
                return BadRequest(_response);
            }
            Category? category = null;
            try
            {
                category = await _UnitOfWork.CategoryRepo.Get(x => x.Id == id);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Errors.Add(ex.Message);
                _response.Errors.Add("Error occured during the data fetching ");
                _response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
            if (category == null)
            {
                _response.StatusCode = System.Net.HttpStatusCode.NotFound;
                _response.Errors.Add("Il n'y a aucune Category correspondant aux données fournies");
                _response.IsSuccess = false;
                return NotFound(_response);
            }
            _response.Result = category;
            _response.StatusCode = System.Net.HttpStatusCode.OK;
            _response.IsSuccess = true;
            return Ok(_response);
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse>> CreateCategory([FromForm] CategoryCreateDto categoryDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Category category = _Mapper.Map<Category>(categoryDto);
                    await _UnitOfWork.CategoryRepo.Add(category);
                    await _UnitOfWork.Save();

                    _response.IsSuccess = true;
                    _response.StatusCode = HttpStatusCode.Created;
                    _response.Result = category;
                    return CreatedAtRoute("GetCategory", new { id = category.Id }, _response);
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
        [HttpPut("{id:int}")]
        [Authorize(Roles = SD.Role_Admin)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse>> UpdateCategory([FromForm] CategoryUpdateDto categoryDto , int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (id == 0 || id != categoryDto.Id)
                    {
                        _response.IsSuccess = false;
                        _response.Errors.Add("L'identifiant n'est pas valide !!");
                        _response.StatusCode = HttpStatusCode.BadRequest;
                        return BadRequest(_response);
                    }
                    var categoryDb = await _UnitOfWork.CategoryRepo.Get(x => x.Id == id);
                    if (categoryDb == null)
                    {
                        _response.IsSuccess = false;
                        _response.Errors.Add("category est introuvable !!");
                        _response.StatusCode = HttpStatusCode.NotFound;
                        return NotFound(_response);
                    }
                    categoryDb = _Mapper.Map<Category>(categoryDto);
                    _UnitOfWork.CategoryRepo.Update(categoryDb);
                    await _UnitOfWork.Save();
                    _response.IsSuccess = true;
                    _response.StatusCode = HttpStatusCode.OK;
                    _response.Result = categoryDb;
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

        [HttpDelete("{id:int}")]
        [Authorize(Roles = SD.Role_Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> DeleteCategory(int id)
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
                Category category = await _UnitOfWork.CategoryRepo.Get(x => x.Id == id);
                if (category == null)
                {
                    _response.IsSuccess = false;
                    _response.Errors.Add("category est introuvable !!");
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                //Thread.Sleep(2000);
                //you should add a warning in the front because deleting a category gonna cause deleting all realestates of it
                _UnitOfWork.CategoryRepo.Remove(category);
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
    }
}
