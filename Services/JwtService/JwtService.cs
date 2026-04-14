using BAU_PROTO.Persistence;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BAU_PROTO.Services.JwtService
{
    public class JwtService
    {
        private readonly string _key;
        private readonly IConfiguration _config;


        public JwtService(IConfiguration config)
        {
            _config = config;
            _key = Environment.GetEnvironmentVariable(ConstantConfig.Sec) ??
                throw new InvalidOperationException("Error server security config, go ask server owner");
        }

        public string GenerateToken(string username, string role)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }

        public string RenewToken(string token,
            DateTime expiredDate)
        {

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token); ;
            
            var username = jwtToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var claimNames = jwtToken?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
            var role = jwtToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (username == null || claimNames == null || role == null)
            {
                throw new JwtException(JwtErrorType.InvalidToken);
            }

            if (expiredDate < DateTime.Now)
            {
                throw new JwtException(JwtErrorType.RefreshTokenExpired);
            }

            return this.GenerateToken(username, role);
        }


    }
}
