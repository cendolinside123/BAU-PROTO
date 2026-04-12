using BAU_PROTO.Services.AuthService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BAU_PROTO.Controllers.AuthController
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly AuthService _authService;
        public AuthController(AppDbContext context)
        {
            _context = context;
            _authService = new AuthServiceImpl(_context);
        }

        [HttpPost("register")]
        public ActionResult<Object> RegisterUser(RegisterRequestDto inputRegister)
        {
            try
            {
                var _ = _authService.RegisterUser(inputRegister).Result;
                var response = new
                {
                    message = "register success"
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public ActionResult<Object> LoginUser(LoginRequestDto inputLogin)
        {
            try
            {
                var token = _authService.Login(inputLogin).Result;
                var response = new
                {
                    message = "login success",
                    data = new
                    {
                        access_token = token.ToTuple().Item1,
                        refresh_token = token.ToTuple().Item2,
                        user_info = new
                        {
                            email = token.ToTuple().Item3.Email,
                            role = token.ToTuple().Item3.Role,
                            created_at = token.ToTuple().Item3.CreatedAt

                        }
                    }
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("refresh")]
        [Authorize]
        public ActionResult<Object> refreshToken()
        {
            try
            {
                var refreshToken = Request.Headers["refreshToken"].ToString();
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var newAccessToken = _authService.RefreshToken(refreshToken, accessToken).Result;
                var response = new
                {
                    message = "refresh token success",
                    data = new
                    {
                        access_token = newAccessToken
                    }
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public ActionResult<Object> logout()
        {
            try
            {
                var refreshToken = Request.Headers["refreshToken"].ToString();
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var doLogout = _authService.Logout(refreshToken).Result;
                var response = new
                {
                    message = "logout success"
                };
                return Ok(response);
            }
            catch (Exception ex)
            {

                return BadRequest(new { message = ex.Message });
            }
        }

        //[HttpPost("test")]
        //[Authorize]
        //public ActionResult<Object> test()
        //{
        //    var refreshToken = Request.Headers["refreshToken"].ToString();
        //    var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        //    try
        //    {
        //        _ = _authService.RefreshTokenValidation(refreshToken).Result;
        //        var response = new
        //        {
        //            message = "test success",
        //            data = new
        //            {
        //                access_token = accessToken,
        //                refresh_token = refreshToken
        //            }
        //        };
        //        return Ok(response);
        //    }
        //    catch (Exception ex) {
        //        return BadRequest(new { message = "test failed" });
        //    }
             
        //}
    }
}
