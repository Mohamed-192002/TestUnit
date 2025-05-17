using Microsoft.AspNetCore.SignalR.Client;

namespace ConsoleApp1.methods;

public class InAppHubMethods
{
    private readonly HubConnection _connection;
    private readonly string _email;

    public InAppHubMethods(HubConnection connection, string email)
    {
        _connection = connection;
        _email = email;
    }

    public async Task CreateRoom()
    {
        var roomDto = new
        {
            Name = "TestRoom_" + Guid.NewGuid(),
            IsPrivate = false
        };

        await _connection.InvokeAsync("CreateRoom", roomDto);

        Console.WriteLine($"[{_email}] ✅ Room created successfully");
    }
}
