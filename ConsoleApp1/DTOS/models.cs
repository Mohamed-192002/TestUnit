namespace ConsoleApp1.DTOS;

public class Tokens
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public string BackgroundHubToken { get; set; }
}

public class AuthData
{
    public Tokens Tokens { get; set; }
    public int AccountStatus { get; set; }
}

public class AuthResponse
{
    public string Message { get; set; }
    public bool Success { get; set; }
    public int StatusCode { get; set; }
    public AuthData Data { get; set; }
}
