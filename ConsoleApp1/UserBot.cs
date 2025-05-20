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
        private Dictionary<string, List<Func<Task>>> hubActions;
        private InAppHubMethods _inAppHubMethods;
        private MessagingStringHubMethods _messagingStringHubMethods;
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

            var random = new Random();

            var hubNames = hubActions.Keys.ToList();
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
                    case "MessagingStringHub":
                        _messagingStringHubMethods = new MessagingStringHubMethods(_connection, Email);
                        break;
                }

                var actions = hubActions[hubName];
                TimeSpan duration = TimeSpan.FromMinutes(2);
                DateTime endTime = DateTime.Now.Add(duration);
                while (DateTime.Now < endTime)
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
                      () => _inAppHubMethods.CreateRoom(3)
                    },
                ["MessagingStringHub"] = new List<Func<Task>>
                    {
                      () => _messagingStringHubMethods.SendMessage(3)
                    },
            };
        }
        #endregion
    }

}