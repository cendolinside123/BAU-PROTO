using BAU_PROTO.Persistence;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace BAU_PROTO.Services.AuthService
{
    public class AuthServiceImpl : AuthService
    {

        private readonly AppDbContext _context;

        private readonly JwtService.JwtService _jwtService;
        private readonly string _key;
        private readonly string _iv;

        public AuthServiceImpl(AppDbContext context)
        {
            _context = context;
            var _config = new ConfigurationBuilder().AddJsonFile(ConstantConfig.GetAppConfig()).Build();
            _jwtService = new JwtService.JwtService(_config);
            _key = Environment.GetEnvironmentVariable(ConstantConfig.Key) ?? throw new InvalidOperationException("Error server security config, go ask server owner"); ;
            _iv = Environment.GetEnvironmentVariable(ConstantConfig.IV) ?? throw new InvalidOperationException("Error server security config, go ask server owner");
        }

        public async Task<(string token, string refreshToken, Users userInfo)> Login(LoginRequestDto loginRequest)
        {
            var validationErrors = loginRequest.InputValidation();
            if (validationErrors.Count > 0)
            {
                throw new ArgumentException(string.Join(", ", validationErrors));
            }

            var getUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginRequest.Email);

            if (getUser == null) {
                throw new ArgumentException("Email not found");
            }

            var getPasssword = SecurityEncrypt.Decrypt(getUser.PasswordHash, _key, _iv);

            if (loginRequest.DecryptPassword() != getPasssword)
            {
                throw new ArgumentException("Invalid email or password");
            }
            var token = _jwtService.GenerateToken(getUser.Email, getUser.Role);
            var refreshToken = _jwtService.GenerateRefreshToken();
            getUser.RefreshToken = refreshToken;
            getUser.RefreshTokenExpiresAt = DateTime.UtcNow.Date.AddDays(1);
            getUser.LoginAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();

                return (token, refreshToken, getUser);
            } catch (Exception ex)
            {
                throw new ArgumentException("Error when login, try again later");
            }
        }

        public async Task<string> RefreshToken(string refreshToken, string accessToken)
        {
            try
            {
                var getUser = await this.RefreshTokenValidation(refreshToken);
                var newAccessToken = _jwtService.RenewToken(accessToken, getUser.expiredDate);
                if (newAccessToken == null)
                {
                    throw new ArgumentException("Invalid access token, re-login");
                } else
                {
                    return newAccessToken;
                }
            }
            catch (JwtException ex)
            {
                throw new ArgumentException(string.Format("Invalid access token, re-login: {0}", ex.Message));
            }
        }

        public async Task<int> RegisterUser(RegisterRequestDto registerRequest)
        {
            var validationErrors = registerRequest.InputValidation();

            if (validationErrors.Count > 0)
            {
                throw new ArgumentException(string.Join(", ", validationErrors));
            }

            var passwordHash = SecurityEncrypt.Encrypt(registerRequest.DecryptPassword(), _key, _iv);
            var user = new Users
            {
                Email = registerRequest.Email,
                PasswordHash = passwordHash,
                Role = registerRequest.Role
            };

            _context.Users.Add(user);

            try
            {
                await _context.SaveChangesAsync();
                return user.Id;
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is MySqlException mysqlEx &&
                    mysqlEx.Number == 1062)
                {
                    throw new ArgumentException("Email already exists");
                }

                throw new ArgumentException("Error when register, try again later"); // rethrow other errors
            }
        }

        public async Task<(int userId, DateTime expiredDate)> RefreshTokenValidation(string refreshToken)
        {
            var getUser = await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

            if (getUser == null || getUser.RefreshTokenExpiresAt == null)
            {
                throw new ArgumentException("Invalid refresh token, failed get user token info");
            }

            if (getUser.RefreshTokenExpiresAt < DateTime.UtcNow)
            {
                throw new ArgumentException("Refresh token expired, re-login");
            }
            return (getUser.Id, getUser.RefreshTokenExpiresAt.Value);
        }

        public async Task<int> Logout(string refreshToken)
        {
            try
            {
                var getUser = await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

                if (getUser == null)
                {
                    return 0; // No user found with the provided refresh token, consider it as already logged out
                }

                getUser.RefreshToken = null;
                getUser.RefreshTokenExpiresAt = null;
                _context.Users.Update(getUser);
                await _context.SaveChangesAsync();
                return getUser.Id;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Error when logout, try again later");
            }
        }
    }
}
