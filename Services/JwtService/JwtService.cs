using BAU_PROTO.Persistence;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
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

        public string GenerateToken(string username, string role, string refreshToken)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("refreshToken", refreshToken)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var now = DateTime.UtcNow;
            var oneHour = now.AddHours(1);
            var midnight = now.Date.AddDays(1).AddSeconds(-1);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: oneHour < midnight ? oneHour : midnight,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }

        public (string token, string email) RenewToken(string token, string refreshToken)
        {

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = GetUserInfoFromToken(token);

            if (jwtToken.email is not string username) throw new JwtException(JwtErrorType.InvalidToken);
            if (jwtToken.claimNames is not string claimNames) throw new JwtException(JwtErrorType.InvalidToken);
            if (jwtToken.role is not string role) throw new JwtException(JwtErrorType.InvalidToken);
            if (jwtToken.refreshToken is not string refreshTokenClaim) throw new JwtException(JwtErrorType.InvalidToken);
            if (jwtToken.expiredDate is not DateTime expiredDateFromToken) throw new JwtException(JwtErrorType.InvalidToken, "Invalid expired date");

            if (refreshTokenClaim != refreshToken)
            {
                throw new JwtException(JwtErrorType.RefreshTokenExpired);
            }

            if (expiredDateFromToken.Date.AddDays(1) <= DateTime.UtcNow)
            {
                throw new JwtException(JwtErrorType.RefreshTokenExpired);
            }

            return (this.GenerateToken(username, role, GenerateRefreshToken()), username);
        }

        public (string? email, string? role, string? refreshToken, string? claimNames, DateTime? expiredDate) GetUserInfoFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var email = jwtToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var role = jwtToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var refreshTokenClaim = jwtToken?.Claims.FirstOrDefault(c => c.Type == "refreshToken")?.Value;
            var claimNames = jwtToken?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
            var expiredDate = jwtToken?.ValidTo;

            return (email, role, refreshTokenClaim, claimNames, expiredDate);
        }
    }
}
