using System.Net.Http.Headers;
using System.Net.Http.Json;
using ConsoleApp1.DTOS;
using ConsoleApp1.methods;
using Microsoft.AspNetCore.SignalR.Client;

namespace ConsoleApp1
{
    public class UserBot
    {
        #region Properties
        //  const string BaseUrl = "http://188.245.96.46:80";
        const string BaseUrl = "http://localhost:5031";
        public string Email { get; }
        public string Password { get; }
        public int MobileId { get; }
        private HubConnection _connection;
        private readonly HttpClient _httpClient = new();
        private Dictionary<string, List<Func<Task>>> hubActions;
        private InAppHubMethods _inAppHubMethods;
        #endregion
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

                switch (hubName)
                {
                    case "inAppHub":
                        _inAppHubMethods = new InAppHubMethods(_connection, Email);
                        break;
                    //case "MessagingStringHub":
                    //    _messagingStringHub = new MessagingStringHub(_connection, Email);
                    //    break;
                }

                var actions = hubActions[hubName];

                for (int i = 0; i < 5; i++)
                {
                    var action = actions[random.Next(actions.Count)];
                    await action();

                    await Task.Delay(random.Next(500, 1500));
                }

                await _connection.StopAsync();
                Console.WriteLine($"🛑 Disconnected from {hubName}");
            }
        }

        #region Privet Methods
        private void InitializeHubActions()
        {
            hubActions = new Dictionary<string, List<Func<Task>>>
            {
                ["inAppHub"] = new List<Func<Task>>
                    {
                      () => _inAppHubMethods.CreateRoom()
                    },
                //["MessagingStringHub"] = new List<Func<Task>>
                //{
                //},
                //["otpHub"] = new List<Func<Task>>
                //{
                //},
                //["testHub"] = new List<Func<Task>>
                //{
                //},
                //["callHub"] = new List<Func<Task>>
                //{
                //}
            };
        }
        #endregion
    }

}