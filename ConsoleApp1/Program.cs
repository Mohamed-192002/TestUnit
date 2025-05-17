namespace ConsoleApp1
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var bots = new List<UserBot>
                    {
                        new UserBot("cinae0w88qwz@zv7vzuv.com","12345678",31),
                        //new UserBot("Bot_Mona", "",3),
                        //new UserBot("Bot_Karim", "", 3),
                        //new UserBot("Bot_Sara", "", 3)
                    };

            var tasks = bots.Select(bot => bot.StartAsync());
            await Task.WhenAll(tasks);
        }
    }
}
