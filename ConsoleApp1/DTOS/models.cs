namespace ConsoleApp1.DTOS;

public class AuthResponse<T> where T : class
{
    public string Message { get; set; }
    public bool Success { get; set; }
    public int StatusCode { get; set; }
    public T Data { get; set; }
}

public class AuthData
{
    public Tokens Tokens { get; set; }
    public int AccountStatus { get; set; }
}
public class Tokens
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public string BackgroundHubToken { get; set; }
}

public class SignalRKeyData
{
    public string Key { get; set; }
}


