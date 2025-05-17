using System.Net.Http.Headers;
using System.Net.Http.Json;
using ConsoleApp1.DTOS;
using ConsoleApp1.methods;
using Microsoft.AspNetCore.SignalR.Client;

namespace ConsoleApp1
{
    public class UserBot
    {
        //  const string BaseUrl = "http://188.245.96.46:80";
        const string BaseUrl = "http://localhost:5031";
        public string Email { get; }
        public string Password { get; }
        public int MobileId { get; }
        private HubConnection _connection;
        private readonly HttpClient _httpClient = new();
        private Dictionary<string, List<Func<Task>>> hubActions;

        public UserBot(string email, string password, int mobileOd)
        {
            Email = email;
            Password = password;
            MobileId = mobileOd;
        }
        public async Task StartAsync()
        {
            var httpClient = new HttpClient();
            InitializeHubActions();

            var hubNames = hubActions.Keys.ToList();
            var random = new Random();

            foreach (var hubName in hubNames)
            {
                var loginService = new ConnectHubService(httpClient, Email, Password, MobileId, BaseUrl, hubName);
                var connection = await loginService.ConnectAsync();
                if (connection == null)
                {
                    Console.WriteLine($"❌ Failed to connect to {hubName}");
                    continue;
                }

                _connection = connection;
                Console.WriteLine($"✅ Connected to {hubName}");

                var actions = hubActions[hubName];

                for (int i = 0; i < 5; i++)
                {
                    var action = actions[random.Next(actions.Count)];
                    await action();
                    var inAppHub = new InAppHubMethods(httpClient, Email, BaseUrl);

                    // استدعاء الميثود
                    await inAppHub.CreateRoom();
                    await Task.Delay(random.Next(500, 1500));
                }

                await _connection.StopAsync();
                Console.WriteLine($"🛑 Disconnected from {hubName}");
            }
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


        private void InitializeHubActions()
        {
            hubActions = new Dictionary<string, List<Func<Task>>>
            {
                ["inAppHub"] = new List<Func<Task>>
                    {
                        CreateRoom,
                    },
                ["MessagingStringHub"] = new List<Func<Task>>
                    {
                    },
                ["otpHub"] = new List<Func<Task>>
                    {
                    },
                ["testHub"] = new List<Func<Task>>
                    {
                    },
                ["callHub"] = new List<Func<Task>>
                    {
                    }
            };
        }

    }

}