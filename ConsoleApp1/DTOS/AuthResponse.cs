namespace ConsoleApp1.DTOS;

public class AuthResponse<T> where T : class
{
    public string Message { get; set; }
    public bool Success { get; set; }
    public int StatusCode { get; set; }
    public T Data { get; set; }
}


