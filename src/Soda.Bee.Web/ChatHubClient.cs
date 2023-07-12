using System.Collections.Concurrent;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace Soda.Bee.Web;

public static class ChatHubClient
{
    private static HubConnection? _hubConnection;

    public static void Configure(HubConnection hubConnection)
    {
        _hubConnection = hubConnection;
    }

    public static HashSet<string> Groups = new HashSet<string>();
    public static Dictionary<string, ConcurrentBag<string>> MessageBox = new();

    public static void OnReceiveMessage(Action<string, string> action)
    {
        _hubConnection.On<string, string>("ReceiveMessage", action);
    }
    
    public static void OnReceiveGroupMessage(Action<string, string> action)
    {
        _hubConnection.On<string, string>("ReceiveGroupMessage", action);
    }
    
    public static void OnCreateGroup(Action<string> action)
    {
        _hubConnection.On<string>("CreateGroup", action);
    }
    
    public static async Task CreateGroup(string groupName)
    {
        await _hubConnection.SendAsync("CreateGroup", groupName);
    }
    
    public  static async Task SendMessage(string user, string message)
    {
        await _hubConnection.SendAsync("CreateGroup", user, message);
    }
    
    public static async Task SendGroupMessage(string groupName, string message)
    {
        await _hubConnection.SendAsync("BroadcastGroupMessage", groupName, message);
    }
}