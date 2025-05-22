using ConsoleApp1.methods;

namespace ConsoleApp1
{
    public class MessagingModule : IBotModule
    {
        private readonly MessagingStringHubMethods _messaging;
        private readonly List<long> _targetUserIds;
        private readonly List<string> _sampleMessages;
        private readonly Random _random;
        public MessagingModule(MessagingStringHubMethods messaging)
        {
            _messaging = messaging;
            _random = new Random();

            // ممكن تجيب دي من config أو API لاحقًا
            _targetUserIds = new List<long> { 1001, 1002, 1003 };
            _sampleMessages = new List<string>
                {
                    "السلام عليكم 🌟",
                    "إزيك عامل ايه؟",
                    "فيه أخبار جديدة؟",
                    "تجربة بوت إرسال رسالة 🔄",
                    "هل تم استلام الرسالة؟ 📬"
                };
        }

        public async Task ExecuteAsync()
        {
            // اختيار مستخدم ورسالة عشوائية
            long targetUserId = _targetUserIds[_random.Next(_targetUserIds.Count)];
            string message = _sampleMessages[_random.Next(_sampleMessages.Count)];

            // Notify typing
            await _messaging.NotifyTyping(targetUserId);

            // Send message
            await _messaging.SendMessage(targetUserId, message);

            // Get unread messages
            await _messaging.GetUnreadMessages();

            Console.WriteLine("📨 MessagingModule done");
        }
    }

}
