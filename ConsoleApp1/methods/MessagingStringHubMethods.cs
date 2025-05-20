using Microsoft.AspNetCore.SignalR.Client;

namespace ConsoleApp1.methods;

public class MessagingStringHubMethods
{
    private readonly HubConnection _connection;
    private readonly string _email;

    public MessagingStringHubMethods(HubConnection connection, string email)
    {
        _connection = connection;
        _email = email;
    }

    public async Task SendMessage(int s)
    {
        var roomDto = new
        {
            Name = "TestRoom_" + Guid.NewGuid(),
            IsPrivate = false
        };

        await _connection.InvokeAsync("SendMessage", roomDto);

        Console.WriteLine($"[{_email}] ✅ Room created successfully");
    }
}
