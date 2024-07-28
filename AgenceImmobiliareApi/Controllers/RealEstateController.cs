using AgenceImmobiliareApi.Data;
using AgenceImmobiliareApi.Models;
using AgenceImmobiliareApi.Models.DTOs;
using AgenceImmobiliareApi.Repository.IRepository;
using AgenceImmobiliareApi.Utility;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Xml;

namespace AgenceImmobiliareApi.Controllers
{
    [Route("api/RealEstate")]
    [ApiController]
    public class RealEstateController : ControllerBase
    {
        private IUnitOfWork _UnitOfWork;
        private ApiResponse _response;
        private readonly IWebHostEnvironment _WebHostEnvironment;
        private readonly IMapper _mapper;

        public RealEstateController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment, IMapper mapper)
        {
            _UnitOfWork = unitOfWork;
            _response = new ApiResponse();
            _WebHostEnvironment = webHostEnvironment;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> GetAll()
        {
            try
            {
                IEnumerable<RealEstate> result = await _UnitOfWork.RealEstateRepo.GetAll(includeProperties: "Images,Category,Addresse");
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

        [HttpGet("{id:int}", Name = "GetRealEstate")] //Name for redirection
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
            RealEstate? realEstate = null;
            try
            {
                realEstate = await _UnitOfWork.RealEstateRepo.Get(x => x.Id == id, includeProperties: "Images,Category,Addresse");
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Errors.Add(ex.Message);
                _response.Errors.Add("Error occured during the data fetching ");
                _response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
            if (realEstate == null)
            {
                _response.StatusCode = System.Net.HttpStatusCode.NotFound;
                _response.Errors.Add("Il n'y a aucune Immobilier correspondant aux données fournies");
                _response.IsSuccess = false;
                return NotFound(_response);
            }
            _response.Result = realEstate;
            _response.StatusCode = System.Net.HttpStatusCode.OK;
            _response.IsSuccess = true;
            return Ok(_response);
        }

        [HttpGet("Category/{category}")] //Name for redirection
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> GetByCategory(string category)
        {
            if (string.IsNullOrEmpty(category))
            {
                _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.Errors.Add("category has no value defined!!");
                return BadRequest(_response);
            }

            try
            {
                Category catDb = await _UnitOfWork.CategoryRepo.Get(x => x.CategoryName == category);
                var result = await _UnitOfWork.RealEstateRepo.GetAll(x => x.CategoryId == catDb.Id, includeProperties: "Images,Category,Addresse");
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

        [HttpGet("Offre/{OffreType}")] //Name for redirection
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> GetByOffre(string OffreType)
        {
            if (string.IsNullOrEmpty(OffreType))
            {
                _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.Errors.Add("Offre has no value defined!!");
                return BadRequest(_response);
            }

            try
            {
                var result = await _UnitOfWork.RealEstateRepo.GetAll(x => x.OffreType == OffreType, includeProperties: "Images,Category,Addresse");
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
        [HttpGet("Latest")] //Name for redirection
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> GetLatest()
        {
            try
            {
                var result = await _UnitOfWork.AppDbContext().RealEstates.Include(x => x.Images).Include(x => x.Category).Include(x => x.Addresse).OrderByDescending(x => x.PostingDate).Take(10).ToListAsync();

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
        [HttpGet("SearchBy")] //Name for redirection
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> GetBySearch([FromForm] RechercheCritiria SearchObj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _UnitOfWork.RealEstateRepo.GetAll(x=> x.OffreType == SearchObj.OffreType && x.Price >= SearchObj.PrixMin && x.Price <= SearchObj.PrixMax,  includeProperties:"Images,Category,Addresse");
                    if (result != null)
                    {
                        if (!string.IsNullOrEmpty(SearchObj.Category))
                        {
                            result = result.Where(x => x.Category.CategoryName == SearchObj.Category);
                        }
                        if (SearchObj.Rooms != 0)
                            result = result.Where(x => x.Room == SearchObj.Rooms);
                        if (SearchObj.floor != 0)
                            result = result.Where(x => x.Floor == SearchObj.floor);

                        if (string.IsNullOrEmpty(SearchObj.Wilaya))
                            result = result.Where(x => x.Addresse.Wilaya == SearchObj.Wilaya);
                        if (string.IsNullOrEmpty(SearchObj.Ville))
                            result = result.Where(x => x.Addresse.Ville == SearchObj.Ville);
                        if (string.IsNullOrEmpty(SearchObj.Rue))
                            result = result.Where(x => x.Addresse.Rue == SearchObj.Rue);

                        if (SearchObj.surfaceMax != 0)
                            result = result.Where(x => x.Surface <= SearchObj.surfaceMax);
                        if (SearchObj.surfaceMin != 0)
                            result = result.Where(x => x.Surface >= SearchObj.surfaceMin);
                    }
                    _response.IsSuccess = true;
                    _response.StatusCode = HttpStatusCode.OK;
                    _response.Result = result ?? [];
                    return Ok(result);
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

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse>> CreateRealEstate([FromForm] RealEstateCreateDto RealEstateDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    RealEstate RealEstate = _mapper.Map<RealEstate>(RealEstateDto);
                    RealEstate.PostingDate = DateTime.Now;
                    RealEstate.UpdatedDate = DateTime.Now;

                    //update the nbr of real estate in the specific category
                    Category category = await _UnitOfWork.CategoryRepo.Get(x => x.Id ==  RealEstate.CategoryId);
                    category.NbREstate += 1;
                    _UnitOfWork.CategoryRepo.Update(category);
                    await _UnitOfWork.Save();

                    RealEstate.NbImage = RealEstateDto.ImagesFiles == null ? 0 : RealEstateDto.ImagesFiles.Count;
                    //prepare the addresse
                    Addresse addresse = new ()
                    {
                        Wilaya = RealEstateDto.Wilaya ?? "",
                        Ville = RealEstateDto.Ville,
                        Rue = RealEstateDto.Rue,
                        PostalCode = RealEstateDto.PostalCode,
                    };
                    await _UnitOfWork.AddresseRepo.Add(addresse);
                    await _UnitOfWork.Save();
                    RealEstate.AddressId = addresse.Id;

                    await _UnitOfWork.RealEstateRepo.Add(RealEstate);
                    await _UnitOfWork.Save();

                    //add the images
                    if (RealEstateDto.ImagesFiles != null)
                    {
                        var images = _UnitOfWork.ImageRepo.UpsertImagesToFolder(_WebHostEnvironment, RealEstate.Id, RealEstateDto.ImagesFiles);
                        await _UnitOfWork.ImageRepo.AddRange(images);
                        await _UnitOfWork.Save();
                    }
                    _response.IsSuccess = true;
                    _response.StatusCode = HttpStatusCode.Created;
                    _response.Result = RealEstate;
                    return CreatedAtRoute("GetRealEstate", new { id = RealEstate.Id }, _response);
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
        public async Task<ActionResult<ApiResponse>> UpdateRealEstate([FromForm]  RealEstateUpdateDto RealEstateDto, int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (id == 0 || id != RealEstateDto.Id)
                    {
                        _response.IsSuccess = false;
                        _response.Errors.Add("L'identifiant n'est pas valide !!");
                        _response.StatusCode = HttpStatusCode.BadRequest;
                        return BadRequest(_response);
                    }
                    var RealEstateDb = await _UnitOfWork.RealEstateRepo.Get(x => x.Id == id ,includeProperties:"Images");
                    if (RealEstateDb == null)
                    {
                        _response.IsSuccess = false;
                        _response.Errors.Add("l'immobilier est introuvable !!");
                        _response.StatusCode = HttpStatusCode.NotFound;
                        return NotFound(_response);
                    }
                   RealEstateDto.AddressId = RealEstateDb.AddressId;
                    var datePosting = RealEstateDb.PostingDate;
                    var nbImages = RealEstateDb.NbImage;
                    if(RealEstateDb.CategoryId != RealEstateDto.CategoryId)
                    {
                        Category catAdd = await _UnitOfWork.CategoryRepo.Get(x =>x.Id == RealEstateDto.CategoryId);
                        Category catDel = await _UnitOfWork.CategoryRepo.Get(x => x.Id == RealEstateDb.CategoryId);
                        catAdd.NbREstate += 1;
                        catDel.NbREstate -= 1;
                    }
                    RealEstateDb = _mapper.Map<RealEstate>(RealEstateDto);
                    RealEstateDb.UpdatedDate = DateTime.Now;
                    RealEstateDb.PostingDate = datePosting;
                    RealEstateDb.NbImage = nbImages;
                    RealEstateDb.NbImage = RealEstateDto.ImagesFiles == null ? RealEstateDb.NbImage : RealEstateDb.NbImage + RealEstateDto.ImagesFiles.Count;

                    //prepare the addresse
                    Addresse addresse = new()
                    {
                        Id = RealEstateDb.AddressId,
                        Wilaya = RealEstateDto.Wilaya ?? "",
                        Ville = RealEstateDto.Ville,
                        Rue = RealEstateDto.Rue,
                        PostalCode = RealEstateDto.PostalCode,
                    };
                    _UnitOfWork.AddresseRepo.Update(addresse);
                    await _UnitOfWork.Save();
                    if (RealEstateDto.ImagesFiles != null)
                    {
                        var images = _UnitOfWork.ImageRepo.UpsertImagesToFolder(_WebHostEnvironment,RealEstateDb.Id, RealEstateDto.ImagesFiles );
                        RealEstateDb.Images = images;
                    }
                    //else
                    //{
                    // var images = _UnitOfWork.ImageRepo.UpsertImagesToFolde(_WebHostEnvironment,RealEstateId RealEstateDb.Id , Deleted: true);
                    //    RealEstateDb.Images = images;
                    //}
                    _UnitOfWork.RealEstateRepo.Update(RealEstateDb);
                    await _UnitOfWork.Save();
                    _response.IsSuccess = true;
                    _response.StatusCode = HttpStatusCode.OK;
                    _response.Result = RealEstateDb;
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
        public async Task<ActionResult<ApiResponse>> DeleteRealEState(int id)
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
                RealEstate realEstate = await _UnitOfWork.RealEstateRepo.Get(x => x.Id == id);
                if (realEstate == null)
                {
                    _response.IsSuccess = false;
                    _response.Errors.Add("l'immobilier est introuvable !!");
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }
                Addresse addresse = await _UnitOfWork.AddresseRepo.Get(x => x.Id == realEstate.AddressId);
                //Thread.Sleep(2000);
                
                if(addresse != null)
                    _UnitOfWork.AddresseRepo.Remove(addresse);
                Category category = await _UnitOfWork.CategoryRepo.Get(x => x.Id == realEstate.CategoryId);
                category.NbREstate -= 1;
                _UnitOfWork.CategoryRepo.Update(category);

                _UnitOfWork.RealEstateRepo.Remove(realEstate);
                await _UnitOfWork.Save();

                //delete the folder of images
                _ = _UnitOfWork.ImageRepo.UpsertImagesToFolder(_WebHostEnvironment, realEstate.Id, Deleted: true);

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
