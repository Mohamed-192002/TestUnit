using System.Net.Http.Json;
using ConsoleApp1.DTOS;
using Microsoft.AspNetCore.SignalR.Client;

namespace ConsoleApp1
{
    public class UserBot
    {
        const string BaseUrl = "http://188.245.96.46:80";
        public string Email { get; }
        public string Password { get; }
        public int MobileId { get; }
        private HubConnection _connection;
        private readonly HttpClient _httpClient = new();

        public UserBot(string email, string password, int mobileOd)
        {
            Email = email;
            Password = password;
            MobileId = mobileOd;
        }

        public async Task StartAsync()
        {
            _connection = new HubConnectionBuilder()
                .WithUrl($"{BaseUrl}/inAppHub", options =>
                {
                    options.AccessTokenProvider = async () => await GetJwtToken();
                })
                .WithAutomaticReconnect()
                .Build();

            _connection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                Console.WriteLine($"[{Email}] <- {user}: {message}");
            });

            await _connection.StartAsync();

            // Random Behavior
            var actions = new List<Func<Task>>
                {
                    SendMessageToRoom,
                    SendFriendRequest,
                    CreateRoom
                };

            var random = new Random();

            for (int i = 0; i < 10; i++)
            {
                var action = actions[random.Next(actions.Count)];
                await action();
                await Task.Delay(random.Next(500, 2000));
            }

            await _connection.StopAsync();
        }

        private async Task SendMessageToRoom()
        {
            Console.WriteLine($"[{Email}] Sending message...");
            await _connection.InvokeAsync("SendMessage", Email, $"Hello from {Email}");
        }

        private async Task SendFriendRequest()
        {
            Console.WriteLine($"[{Email}] Sending friend request...");

            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/api/friends/request", new
            {
                from = Email,
                to = "TargetUser" 
            });

            Console.WriteLine($"[{Email}] Friend request status: {response.StatusCode}");
        }

        private async Task CreateRoom()
        {
            Console.WriteLine($"[{Email}] Creating room...");

            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/api/rooms", new
            {
                name = $"Room_{Email}_{Guid.NewGuid().ToString().Substring(0, 4)}"
            });

            Console.WriteLine($"[{Email}] Room creation status: {response.StatusCode}");
        }

        private async Task<string> GetJwtToken()
        {
            var loginDto = new
            {
                Email = Email,
                Password = Password
            };
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/auth/login?mobileId={MobileId}", loginDto);
            if (!response.IsSuccessStatusCode)
                return null;

            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
            return result?.Data?.Tokens?.AccessToken;
        }
    }

}