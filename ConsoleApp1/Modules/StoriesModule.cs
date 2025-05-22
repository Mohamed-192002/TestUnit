using ConsoleApp1.methods;

namespace ConsoleApp1
{
    public class StoriesModule : IBotModule
    {
        private readonly MessagingStringHubMethods _messaging;

        public StoriesModule(MessagingStringHubMethods messaging)
        {
            _messaging = messaging;
        }

        public async Task ExecuteAsync()
        {
            await _messaging.GetStories();
            await Task.Delay(500);
            await _messaging.UploadStory();

            Console.WriteLine("📚 StoriesModule done");
        }
    }

}
