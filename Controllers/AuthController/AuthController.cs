using BAU_PROTO.Services.AuthService;
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
    }
}
