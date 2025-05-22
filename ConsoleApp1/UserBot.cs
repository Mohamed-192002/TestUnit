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
      //  const string BaseUrl = "http://localhost:5031";
        public string Email { get; }
        public string Password { get; }
        public int MobileId { get; }
        private HubConnection _connection;
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

            var random = new Random();

            var hubNames = GetModulesPerHub().Keys.ToList();

            foreach (var hubName in hubNames)
            {
                var loginService = new ConnectHubService(httpClient, Email, Password, MobileId, Consts.BaseUrl, hubName);
                var (connection, userId) = await loginService.ConnectAsync();
                if (connection == null)
                {
                    Console.WriteLine($"❌ Failed to connect to {hubName}");
                    continue;
                }
                _connection = connection;

                Console.WriteLine($"✅ Connected to {hubName}");

                switch (hubName)
                {
                    case Consts.inAppHub:
                        _inAppHubMethods = new InAppHubMethods(_connection, Email);
                        break;
                    case Consts.MessagingStringHub:
                        _messagingStringHubMethods = new MessagingStringHubMethods(_connection, Email, userId);
                        break;
                }

                // Get modules
                var modulesPerHub = GetModulesPerHub(); 
                if (modulesPerHub.TryGetValue(hubName, out var moduleFactory))
                {
                    var modules = moduleFactory.Invoke();

                    var duration = TimeSpan.FromMinutes(2);
                    var endTime = DateTime.Now.Add(duration);

                    while (DateTime.Now < endTime)
                    {
                        var module = modules[random.Next(modules.Count)];
                        await module.ExecuteAsync();
                        await Task.Delay(random.Next(1000, 2000));
                    }
                }
                await _connection.StopAsync();
                Console.WriteLine($"🛑 Disconnected from {hubName}");
            }
        }

        #region Privet Methods
        private Dictionary<string, Func<List<IBotModule>>> GetModulesPerHub()
        {
            return new Dictionary<string, Func<List<IBotModule>>>
            {
                [Consts.MessagingStringHub] = () => new List<IBotModule>
                {
                    new MessagingModule(_messagingStringHubMethods),
                    new StoriesModule(_messagingStringHubMethods)
                    // Add more modules like NotificationsModule, ContactsModule, etc.
                },
                [Consts.inAppHub] = () => new List<IBotModule>
                {
                  //  new RoomModule(_inAppHubMethods)
                }
            };
        }

        #endregion
    }

}