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
            _key = _config.GetValue<string>(ConstantConfig.Sec) ??
                throw new InvalidOperationException("Error server security config, go ask server owner");
        }

        public string GenerateToken(string username)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
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

        public string RenewToken(ClaimsIdentity identity,
            DateTime expiredDate,
            string role)
        {
            var username = identity?.FindFirst(ClaimTypes.Name)?.Value;
            var createdAt = identity?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

            if (username == null || createdAt == null)
            {
                throw new InvalidOperationException("Invalid token");
            }

            if (expiredDate < DateTime.Now)
            {
                throw new InvalidOperationException("Refresh token expired, please login again");
            }

            return this.GenerateToken(username);
        }


    }
}
