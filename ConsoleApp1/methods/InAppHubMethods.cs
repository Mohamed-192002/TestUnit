using System.Net.Http.Json;

namespace ConsoleApp1.methods;

public class InAppHubMethods
{
    private readonly HttpClient _httpClient;
    private readonly string _email;
    private readonly string _baseUrl;
    private readonly string _hubName= "inAppHub";

    public InAppHubMethods(HttpClient httpClient, string email, string baseUrl)
    {
        _httpClient = httpClient;
        _email = email;
        _baseUrl = baseUrl;
    }

    public async Task CreateRoom()
    {
        var roomDto = new
        {
            Name = "TestRoom_" + Guid.NewGuid(),
            IsPrivate = false
        };

        var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/{_hubName}/CreateRoom", roomDto);

        Console.WriteLine($"[{_email}] Create room status: {response.StatusCode}");
    }
}
