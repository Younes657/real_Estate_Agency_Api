using AgenceImmobiliareApi.Models;
using AgenceImmobiliareApi.Models.DTOs;
using AgenceImmobiliareApi.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace AgenceImmobiliareApi.Controllers
{
    [Route("api/Authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private IUnitOfWork _UnitOfWork;
        private ApiResponse _response;
        private readonly IMapper _mapper;
        private readonly string SecretKey;
        private UserManager<ApplicationUser> _userManger;
        private RoleManager<IdentityRole> _roleManager;
        public AuthenticationController(IUnitOfWork unitOfWork , IMapper mapper, IConfiguration configuration, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _UnitOfWork = unitOfWork;
            _mapper = mapper;
            _response  = new ApiResponse();
            _roleManager = roleManager;
            _userManger = userManager;
            SecretKey = configuration.GetValue<string>("ApiSettings:SecretKey") ?? throw new NullReferenceException();
        }
        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse>> Login([FromBody] LoginRequestDto model)
        {
            //Console.WriteLine(SecretKey);
            var UserfromDb = _UnitOfWork.AppDbContext().ApplicationUsers.FirstOrDefault(x => x.UserName == model.UserNameEmail || x.Email == model.UserNameEmail);
            if (UserfromDb == null)
            {
                _response.IsSuccess = false;
                _response.Result = new LoginResponseDto();
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Errors.Add("Username ou Email que vous fournissez n'est pas valide");
                return BadRequest(_response);
            }
            var result = await _userManger.CheckPasswordAsync(UserfromDb, model.Password);
            if (result == false)
            {
                _response.IsSuccess = false;
                _response.Result = new LoginResponseDto();
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Errors.Add("Votre mot de passe est incorrect Vérifiez à nouveau");
                return BadRequest(_response);
            }
            //generate the jwt token
            var role = await _userManger.GetRolesAsync(UserfromDb);
            JwtSecurityTokenHandler TokenHandler = new();
            byte[] key = Encoding.ASCII.GetBytes(SecretKey);
            SecurityTokenDescriptor TokenDesc = new()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, UserfromDb.UserName),
                    new Claim(ClaimTypes.Email, UserfromDb.Email),
                    new Claim(ClaimTypes.Role, role.FirstOrDefault()),
                    new Claim("sub", UserfromDb.Id)
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken Token = TokenHandler.CreateToken(TokenDesc);

            LoginResponseDto response = new()
            {
                Email = UserfromDb.Email ?? "",
                Token = TokenHandler.WriteToken(Token),
            };
            if (response.Email == "" || string.IsNullOrEmpty(response.Token))
            { 
                _response.IsSuccess = false;
                _response.Result = new LoginResponseDto();
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Errors.Add("UserName ou Password est Incorrect");
                return BadRequest(_response);
            }
            _response.IsSuccess = true;
            _response.Result = response;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }
    }
}
