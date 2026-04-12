using BAU_PROTO.Persistence;
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

        public Task<(string token, string refreshToken, Users userInfo)> Login(LoginRequestDto loginRequest)
        {
            throw new NotImplementedException();
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
