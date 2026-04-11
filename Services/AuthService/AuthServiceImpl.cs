using BAU_PROTO.Persistence;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace BAU_PROTO.Services.AuthService
{
    public class AuthServiceImpl : AuthService
    {

        private readonly AppDbContext _context;

        private readonly JwtService.JwtService _jwtService;

        public AuthServiceImpl(AppDbContext context)
        {
            _context = context;
            _jwtService = new JwtService.JwtService(new ConfigurationBuilder().AddJsonFile(ConstantConfig.GetAppConfig()).Build());
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


            var user = new Users
            {
                Email = registerRequest.Email,
                PasswordHash = registerRequest.Password,
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
