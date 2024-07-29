using AgenceImmobiliareApi.Models;
using AgenceImmobiliareApi.Models.DTOs;
using AgenceImmobiliareApi.Repository.IRepository;
using AgenceImmobiliareApi.Utility;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AgenceImmobiliareApi.Controllers
{
    [Route("api/BlogArticle")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly IUnitOfWork _UnitOfWork;
        private ApiResponse _response;
        private readonly IMapper _Mapper;
        public BlogController(IUnitOfWork unitofWork , IMapper mapper)
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
                IEnumerable<BlogArticle> result = await _UnitOfWork.BlogArticleRepo.GetAll();
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
        [HttpGet("{id:int}", Name = "GetBlog")]
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
            BlogArticle? blog = null;
            try
            {
                blog = await _UnitOfWork.BlogArticleRepo.Get(x => x.Id == id);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Errors.Add(ex.Message);
                _response.Errors.Add("Error occured during the data fetching ");
                _response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
            if (blog == null)
            {
                _response.StatusCode = System.Net.HttpStatusCode.NotFound;
                _response.Errors.Add("Il n'y a aucune Blog correspondant aux données fournies");
                _response.IsSuccess = false;
                return NotFound(_response);
            }
            _response.Result = blog;
            _response.StatusCode = System.Net.HttpStatusCode.OK;
            _response.IsSuccess = true;
            return Ok(_response);
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse>> CreateBlog([FromForm] BlogCreateDto blogDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    BlogArticle blog = _Mapper.Map<BlogArticle>(blogDto);
                    blog.PublicationDate = DateTime.Now;
                    blog.UpdatedDate = DateTime.Now;
                    await _UnitOfWork.BlogArticleRepo.Add(blog);
                    await _UnitOfWork.Save();

                    _response.IsSuccess = true;
                    _response.StatusCode = HttpStatusCode.Created;
                    _response.Result = blog;
                    return CreatedAtRoute("GetBlog", new { id = blog.Id }, _response);
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
        public async Task<ActionResult<ApiResponse>> UpdateBlog([FromForm] BlogUpdateDto blogDto, int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (id == 0 || id != blogDto.Id)
                    {
                        _response.IsSuccess = false;
                        _response.Errors.Add("L'identifiant n'est pas valide !!");
                        _response.StatusCode = HttpStatusCode.BadRequest;
                        return BadRequest(_response);
                    }
                    var blogDb = await _UnitOfWork.BlogArticleRepo.Get(x => x.Id == id);
                    if (blogDb == null)
                    {
                        _response.IsSuccess = false;
                        _response.Errors.Add("BLog est introuvable !!");
                        _response.StatusCode = HttpStatusCode.NotFound;
                        return NotFound(_response);
                    }
                    var postingDate = blogDb.PublicationDate;
                    blogDb = _Mapper.Map<BlogArticle>(blogDto);
                    blogDb.UpdatedDate = DateTime.Now;
                    blogDb.PublicationDate = postingDate;

                    _UnitOfWork.BlogArticleRepo.Update(blogDb);
                    await _UnitOfWork.Save();
                    _response.IsSuccess = true;
                    _response.StatusCode = HttpStatusCode.OK;
                    _response.Result = blogDb;
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
        public async Task<ActionResult<ApiResponse>> DeleteBlog(int id)
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
                 BlogArticle blog = await _UnitOfWork.BlogArticleRepo.Get(x => x.Id == id);
                if (blog == null)
                {
                    _response.IsSuccess = false;
                    _response.Errors.Add("Blog est introuvable !!");
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                //Thread.Sleep(2000);
                //you should add a warning in the front because deleting a category gonna cause deleting all realestates of it
                _UnitOfWork.BlogArticleRepo.Remove(blog);
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
