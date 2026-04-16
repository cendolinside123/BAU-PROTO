using BAU_PROTO.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace BAU_PROTO.Controllers.ConfigurationController
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfigurationController : ControllerBase
    {


        [HttpPost("configPrivate")]
        public ActionResult<Object> ConfigPrivate()
        {
            try
            {
                string _key = Environment.GetEnvironmentVariable(ConstantConfig.KeyFront); 
                string _iv = Environment.GetEnvironmentVariable(ConstantConfig.IVFront);

                if (_key == null || _iv == null)
                {
                    throw new Exception("Environment variables for key and IV are not set.");
                }

                var response = new
                {
                    S0VZ = _key, // ini key
                    SVY = _iv  // ini IV
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
