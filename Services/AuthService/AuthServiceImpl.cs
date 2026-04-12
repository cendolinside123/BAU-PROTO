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
            _key = _config.GetValue<string>(ConstantConfig.Key) ?? throw new InvalidOperationException("Error server security config, go ask server owner"); ;
            _iv = _config.GetValue<string>(ConstantConfig.IV) ?? throw new InvalidOperationException("Error server security config, go ask server owner");
        }

        public async Task<(string token, string refreshToken, Users userInfo)> Login(LoginRequestDto loginRequest)
        {
            var validationErrors = loginRequest.InputValidation();
            if (validationErrors.Count > 0)
            {
                throw new ArgumentException(string.Join(", ", validationErrors));
            }

            var getUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginRequest.Email);

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

        public Task<string> RefreshToken(string refreshToken)
        {
            throw new NotImplementedException();
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
    }
}
