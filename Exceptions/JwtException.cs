public enum JwtErrorType
{
    InvalidToken,
    ExpiredToken,
    RefreshTokenExpired,
    ConfigurationError
}

public class JwtException : Exception
{
    public JwtErrorType ErrorType { get; }
    public int StatusCode { get; }

    public JwtException(
        JwtErrorType errorType,
        string? message = null
    ) : base(message ?? errorType.ToString())
    {
        ErrorType = errorType;
        StatusCode = GetStatusCode(errorType);
    }

    private static int GetStatusCode(JwtErrorType type) => type switch
    {
        JwtErrorType.InvalidToken => 401,
        JwtErrorType.ExpiredToken => 401,
        JwtErrorType.RefreshTokenExpired => 401,
        JwtErrorType.ConfigurationError => 500,
        _ => 400
    };
}