using ConsoleApp1.DTOS;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.SignalR.Client;

namespace ConsoleApp1;

public class ConnectHubService
{
    private readonly HttpClient _httpClient;
    private readonly string _email;
    private readonly string _password;
    private readonly int _mobileId;
    private readonly string _baseUrl;
    private readonly string _hubName;

    public ConnectHubService(HttpClient httpClient, string email, string password, int mobileId, string baseUrl, string hubName)
    {
        _httpClient = httpClient;
        _email = email;
        _password = password;
        _mobileId = mobileId;
        _baseUrl = baseUrl;
        _hubName = hubName;
    }

    public async Task<HubConnection> ConnectAsync()
    {
        var jwt = await GetJwtToken();
        if (jwt == null)
        {
            Console.WriteLine("Login failed.");
            return null;
        }

        var key = await GetAuthKey(jwt);
        if (key == null)
        {
            Console.WriteLine("Failed to get SignalR key.");
            return null;
        }

        var connection = new HubConnectionBuilder()
            .WithUrl($"{_baseUrl}/{_hubName}?key={key}", options =>
            {
                options.AccessTokenProvider = async () => jwt;
            })
            .WithAutomaticReconnect()
            .Build();

        await connection.StartAsync();
        Console.WriteLine("Connected to SignalR hub.");

        return connection;
    }

    private async Task<string> GetJwtToken()
    {
        var loginDto = new
        {
            Email = _email,
            Password = _password
        };

        var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/auth/login?mobileId={_mobileId}", loginDto);
        if (!response.IsSuccessStatusCode)
            return null;

        var result = await response.Content.ReadFromJsonAsync<AuthResponse<AuthData>>();
        return result?.Data?.Tokens?.AccessToken;
    }

    private async Task<string> GetAuthKey(string accessToken)
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.GetAsync($"{_baseUrl}/signalr/authKey");
        if (!response.IsSuccessStatusCode)
            return null;

        var result = await response.Content.ReadFromJsonAsync<AuthResponse<SignalRKeyData>>();
        return result?.Data?.Key;
    }
}
