using BAU_PROTO.Persistence;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MySqlConnector;

namespace BAU_PROTO.Services.AuthService
{
    public class AuthServiceImpl : AuthService
    {

        private readonly AppDbContext _context;

        private readonly JwtService.JwtService _jwtService;
        private readonly string _key;

        public AuthServiceImpl(AppDbContext context)
        {
            _context = context;
            var _config = new ConfigurationBuilder().AddJsonFile(ConstantConfig.GetAppConfig()).Build();
            _jwtService = new JwtService.JwtService(_config);
            _key = Environment.GetEnvironmentVariable(ConstantConfig.Key) ?? throw new InvalidOperationException("Error server security config, go ask server owner");
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

            var getPasssword = SecurityEncrypt.Decrypt(getUser.PasswordHash, _key);

            if (loginRequest.DecryptPassword() != getPasssword)
            {
                throw new ArgumentException("Invalid email or password");
            }

            var refreshToken = _jwtService.GenerateRefreshToken();
            var token = _jwtService.GenerateToken(getUser.Email, getUser.Role, refreshToken);

            getUser.AccessToken = token;
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
                var newAccessToken = _jwtService.RenewToken(accessToken, refreshToken);
                var getUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == newAccessToken.email);

                if (getUser == null)
                {
                    throw new JwtException(JwtErrorType.ConfigurationError, "Failed get user information");
                }

                getUser.AccessToken = newAccessToken.token;

                await _context.SaveChangesAsync();

                return newAccessToken.token;
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

            var passwordHash = SecurityEncrypt.Encrypt(registerRequest.DecryptPassword(), _key);
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

        public async Task<(int userId, DateTime expiredDate)> TokenValidation(string token)
        {

            var getUserSessionInfo = _jwtService.GetUserInfoFromToken(token);

            if (
                getUserSessionInfo.email == null 
                || getUserSessionInfo.role == null 
                || getUserSessionInfo.refreshToken == null 
                || getUserSessionInfo.claimNames == null 
                || getUserSessionInfo.expiredDate == null
            )
            {
                throw new JwtException(JwtErrorType.InvalidToken, "Invalid refresh token, failed get user token info");
            }

            var getUser = await _context.Users.FirstOrDefaultAsync(u => u.AccessToken == token);

            if (getUser == null || getUser.AccessToken != token)
            {
                throw new JwtException(JwtErrorType.InvalidToken, "Invalid token, failed get user token info");
            }

            var getUserInfo = _jwtService.GetUserInfoFromToken(getUser.AccessToken);

            if (getUserSessionInfo.refreshToken != getUserInfo.refreshToken)
            {
                throw new JwtException(JwtErrorType.InvalidToken, "Invalid refresh token, failed get user token info");
            }

            if (getUserSessionInfo.expiredDate is not DateTime getDateTime) throw new JwtException(JwtErrorType.InvalidToken, "Invalid expired date");

            if (getDateTime.Date.AddDays(1) <= DateTime.UtcNow)
            {
                throw new JwtException(JwtErrorType.RefreshTokenExpired, "Refresh token expired, re-login");
            }
            return (getUser.Id, getDateTime);
        }

        public async Task<int> Logout(string token)
        {
            try
            {
                var getUserInfo = _jwtService.GetUserInfoFromToken(token);

                if (getUserInfo.email == null || getUserInfo.role == null || getUserInfo.refreshToken == null || getUserInfo.claimNames == null)
                {
                    throw new ArgumentException("Invalid refresh token, failed get user token info");
                }

                var getUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == getUserInfo.email);

                if (getUser == null)
                {
                    return 0; // No user found with the provided refresh token, consider it as already logged out
                }

                getUser.AccessToken = null;
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
