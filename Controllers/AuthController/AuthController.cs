using BAU_PROTO.Services.AuthService;
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
    }
}
