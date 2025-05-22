using Microsoft.AspNetCore.SignalR.Client;

namespace ConsoleApp1.methods;

public class MessagingStringHubMethods
{
    private readonly HubConnection _connection;
    private readonly string _email;
    private readonly long _senderId;
    public MessagingStringHubMethods(HubConnection connection, string email, long senderId)
    {
        _connection = connection;
        _email = email;
        _senderId = senderId;
    }

    public async Task NotifyTyping(long targetUserId)
    {
        await _connection.InvokeAsync<object>("NotifyTyping", targetUserId);
        Console.WriteLine($"[{_email}] ✍️ Typing to {targetUserId}");
    }
    public async Task SendMessage(long targetUserId, string message)
    {
        var messageDto = new
        {
            ContentText = message,
            SenderId = _senderId
        };
        await _connection.InvokeAsync<object>("SendMessage", targetUserId, messageDto);
        Console.WriteLine($"[{_email}] 📤 Message sent to {targetUserId}");
    }

    public async Task GetUnreadMessages()
    {
        await _connection.InvokeAsync("GetUnreadMessages");
        Console.WriteLine($"[{_email}] 📩 Checked unread messages");
    }

    public async Task GetStories()
    {
        await _connection.InvokeAsync("GetStories");
        Console.WriteLine($"[{_email}] 👀 Viewed stories");
    }

    public async Task UploadStory()
    {
        await _connection.InvokeAsync("UploadStory", "Sample story content");
        Console.WriteLine($"[{_email}] 📤 Uploaded a story");
    }

}
