namespace BAU_PROTO.Services.AuthService
{
    public interface AuthService
    {
        public Task<(string token, string refreshToken, Users userInfo)> Login(LoginRequestDto loginRequest);
        public Task<string> RefreshToken(string refreshToken, string accessToken);
        public Task<int> RegisterUser(RegisterRequestDto registerRequest);
        public Task<(int userId, DateTime expiredDate)> RefreshTokenValidation(string refreshToken);
        public Task<int> Logout(string refreshToken);
    }
}
