﻿using AgenceImmobiliareApi.Models;
using AgenceImmobiliareApi.Models.DTOs;
using AgenceImmobiliareApi.Repository.IRepository;
using AgenceImmobiliareApi.Services;
using AgenceImmobiliareApi.Utility;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
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
        private IConfiguration _configuration;
        private readonly IEmailService _EmailService;
        public AuthenticationController(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, IConfiguration config,
            IEmailService emailService)
        {
            _UnitOfWork = unitOfWork;
            _mapper = mapper;
            _response = new ApiResponse();
            _roleManager = roleManager;
            _userManger = userManager;
            SecretKey = configuration.GetValue<string>("ApiSetting:SecretKey") ?? throw new NullReferenceException();
            _configuration = config;
            _EmailService = emailService;
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

            string token = GenerateToken(UserfromDb, role.FirstOrDefault());
            string RefreshToken = GenerateRefreshToken();
            UserfromDb.RefreshToken = RefreshToken;
            UserfromDb.RefreshTokenExpiry = DateTime.UtcNow.AddDays(1);
            _UnitOfWork.AppDbContext().ApplicationUsers.Update(UserfromDb);
            await _UnitOfWork.Save();

            LoginResponseDto response = new()
            {
                Email = UserfromDb.Email ?? "",
                Role = role.FirstOrDefault(),
                UserName = UserfromDb.UserName,
                IsAuthenticated = true
            };

            _response.IsSuccess = true;
            _response.Result = response;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

        [HttpGet("RefreshToken")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Refrech()
        {
            string refreshToken = HttpContext.Request.Cookies["refresh-token"] ?? "";
            var UserDb = _UnitOfWork.AppDbContext().ApplicationUsers.FirstOrDefault(x => x.RefreshToken == refreshToken);

            //string Token = HttpContext.Request.Cookies["token"] ?? "";
            //var principal = GetTokenPrincipal(Token);
            if (UserDb != null || UserDb?.RefreshTokenExpiry < DateTime.UtcNow)
            { 
                var role = await _userManger.GetRolesAsync(UserDb);

                string NewJwtToken = GenerateToken(UserDb, role.FirstOrDefault());

                LoginResponseDto response = new()
                {
                    Email = UserDb.Email ?? "",
                    Role = role.FirstOrDefault(),
                    UserName = UserDb.UserName,
                    IsAuthenticated = true
                };

                _response.IsSuccess = true;
                _response.Result = response;
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
                //response.IsLogedIn = true;
                //response.JwtToken = this.GenerateTokenString(identityUser.Email);
                //response.RefreshToken = this.GenerateRefreshTokenString();

                //identityUser.RefreshToken = response.RefreshToken;
                //identityUser.RefreshTokenExpiry = DateTime.Now.AddHours(12);
                //await _userManager.UpdateAsync(identityUser);

            }
            else
            {
                _response.IsSuccess = false;
                _response.Result = new LoginResponseDto();
                _response.StatusCode = HttpStatusCode.Unauthorized;
                _response.Errors.Add("Votre identifiant n'est pas valide, vous devez vous reconnecter");
                return Unauthorized(_response);
            }
        }

        [HttpPost("Logout")]
        [Authorize(Roles = SD.Role_Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse>> Logout()
        {
            var cookieOption = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(-1),
                HttpOnly = true,
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None
            };
            var princpals = GetTokenPrincipal(HttpContext.Request.Cookies["token"] ?? "");
            string username = princpals.Identity.Name;
            HttpContext.Response.Cookies.Delete("token", cookieOption);
            HttpContext.Response.Cookies.Delete("refresh-token", cookieOption);
            var user = await _UnitOfWork.AppDbContext().ApplicationUsers.FirstOrDefaultAsync(x=>x.UserName==username);
            if (user != null)
            {
                user.RefreshToken = "";
                user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(-1);
                _UnitOfWork.AppDbContext().ApplicationUsers.Update(user);
                await _UnitOfWork.Save();
            }
            _response.IsSuccess = true;

            _response.StatusCode = HttpStatusCode.NoContent;

            return NoContent();

        }


        [HttpGet("CheckAuth")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse>> CheckAuth()
        {
            var authToken = HttpContext.Request.Cookies["token"];
            string refreshToken = HttpContext.Request.Cookies["refresh-token"] ?? "";

            if (authToken != null && authToken != "" && !string.IsNullOrEmpty(refreshToken))
            {
                var principal = GetTokenPrincipal(authToken);
                if (principal?.Identity != null)
                {
                    _response.IsSuccess = true;
                    _response.Result = new LoginResponseDto
                    {
                        Email = User.FindFirst(ClaimTypes.Email)?.Value ?? "",
                        Role = User.FindFirst(ClaimTypes.Role)?.Value,
                        UserName = principal.Identity.Name,
                        IsAuthenticated = true
                    };
                    _response.StatusCode = HttpStatusCode.OK;
                    return Ok(_response);
                }
            }

            _response.IsSuccess = false;
            _response.Result = new LoginResponseDto();
            _response.StatusCode = HttpStatusCode.Unauthorized;
            _response.Errors.Add("Votre identifiant n'est pas valide, vous devez vous connecter");
            return Unauthorized(_response);
        }
        [HttpPost("ChangeCredential")]
        [Authorize(Roles = SD.Role_Admin)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse>> ChangeCredential([FromBody] ChangeCredentialDto credentail)
        {
            if (ModelState.IsValid)
            {
                string authToken = HttpContext.Request.Cookies["token"] ?? "";
                if (!string.IsNullOrEmpty(authToken))
                {
                    var principal = GetTokenPrincipal(authToken);
                    if (principal?.Identity != null)
                    {
                        ApplicationUser user = await _userManger.FindByEmailAsync(User.FindFirst(ClaimTypes.Email)?.Value);
                        if (user != null) 
                        {
                            var result = await _userManger.CheckPasswordAsync(user, credentail.OldPassword);
                            if (result)
                            {
                                _ = await _userManger.SetEmailAsync(user, credentail.NewEmail);
                                _ = await _userManger.SetUserNameAsync(user, credentail.UserName);
                                if (!string.IsNullOrEmpty(credentail.NewPassword))
                                {
                                    var res = await _userManger.ChangePasswordAsync(user, credentail.OldPassword, credentail.NewPassword);
                                }
                                _response.IsSuccess = true;
                                _response.StatusCode = HttpStatusCode.OK;
                                _response.Result = new { credentail.UserName, credentail.NewEmail };
                                return Ok(_response);
                            }
                            else
                            {
                                _response.IsSuccess = false;
                                _response.StatusCode = HttpStatusCode.Unauthorized;
                                _response.Errors.Add("Votre Mot de Pass est Incorrect !!");
                            }
                        }
                        else
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = HttpStatusCode.Unauthorized;
                            _response.Errors.Add("Vous étez pas Autorisé de Faire une Modificaton Verifié votre Informations d'Identification !!");
                        }
                    }
                    else
                    {
                        _response.IsSuccess = false;
                        _response.StatusCode = HttpStatusCode.Unauthorized;
                        _response.Errors.Add("Vous étez pas Autorisé de Faire une Modificaton Verifié votre Informations d'Identification !!");
                    }
                }
            }
            else
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Errors = ModelState.SelectMany(x => x.Value.Errors.Select(p => p.ErrorMessage)).ToList();
                return BadRequest(_response);
            }
            return Unauthorized(_response);
        }

        [HttpPost("ForgetPassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse>> ForgetPassword([FromBody] ForgetPasswordDTO ForgetPass)
        {
            if (!ModelState.IsValid)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Errors = ModelState.SelectMany(x => x.Value.Errors.Select(p => p.ErrorMessage)).ToList();
                return BadRequest(_response);
            }
            ApplicationUser? resultdb = await _userManger.FindByEmailAsync(ForgetPass.EmailAddress);
            if (resultdb is  null)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Errors.Add("l'E-mail que vous avez fourni n'existe pas. Vérifiez à nouveau!");
                return BadRequest(_response);
            }
            var clientUrl = HttpContext.Request.Headers["Client-Url"].ToString();
            if (string.IsNullOrEmpty(clientUrl))
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Errors.Add("Une erreur s'est produite pendant le processus, vérifiez à nouveau ou actualisez le site Web");
                return BadRequest(_response);
            }
            var token = await _userManger.GeneratePasswordResetTokenAsync(resultdb);
            Dictionary<string, string?> param = new()
            {
                {"token" ,  token ?? ""},
                {"email" , ForgetPass.EmailAddress }
            };
            string url = QueryHelpers.AddQueryString(clientUrl , param);
            try
            {
                await _EmailService.SendEmailAsync("younesbc2123@gmail.com", "Réinitialiser le mot de passe (cliquez sur l'URL)", url);
                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = "";
                return Ok(_response);
            }
            catch
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.Errors.Add("Erreur : échec de l'envoi de l'e-mail, vérifiez à nouveau");
                return Ok(_response);
            }
        }
        [HttpPost("ResetPassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> ResetPassword([FromBody] ResetPasswordDTO resetPass)
        {
            if (!ModelState.IsValid)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Errors = ModelState.SelectMany(x => x.Value.Errors.Select(p => p.ErrorMessage)).ToList();
                return BadRequest(_response);
            }
            var user = await _userManger.FindByEmailAsync(resetPass.EmailAddress);
            if (user == null)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Errors.Add("l'E-mail que vous avez fourni n'existe pas. Vérifiez à nouveau!");
                return BadRequest(_response);
            }
            var result = await _userManger.ResetPasswordAsync(user, resetPass.Token, resetPass.Password);
            if (!result.Succeeded)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.Errors.Add("échec de la réinitialisation du mot de passe");
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = "";
            return Ok(_response);
        }

        private string GenerateToken(ApplicationUser user, string role)
        {
            JwtSecurityTokenHandler TokenHandler = new();
            byte[] key = Encoding.ASCII.GetBytes(SecretKey);
            SecurityTokenDescriptor TokenDesc = new()
            {
                Subject = new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, role),
                    new Claim("sub", user.Id)
                ]),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken Token = TokenHandler.CreateToken(TokenDesc);
            string TokenString = TokenHandler.WriteToken(Token);
            //adding the token to the cookie 

            HttpContext.Response.Cookies.Append("token", TokenString, new CookieOptions
            {
                Expires = DateTime.UtcNow.AddMinutes(15),
                HttpOnly = true,
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None
            });
            
            return TokenString;
        }
       
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using (var numberGenerator = RandomNumberGenerator.Create())
            {
                numberGenerator.GetBytes(randomNumber);
            }
            string refreshToken = Convert.ToBase64String(randomNumber);
            HttpContext.Response.Cookies.Append("refresh-token", refreshToken, new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(1),
                HttpOnly = true,
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None
            });
            return refreshToken;
        }
        private ClaimsPrincipal? GetTokenPrincipal(string jwtToken)
        {
            var key = Encoding.ASCII.GetBytes(SecretKey);
            var securityKey = new SymmetricSecurityKey(key);

            var validationToken = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = securityKey,
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = true,
            };
            return new JwtSecurityTokenHandler().ValidateToken(jwtToken, validationToken, out _);
        }
    }
}
