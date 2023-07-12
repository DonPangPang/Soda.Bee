using Microsoft.AspNetCore.SignalR;
using Soda.Bee.Core.Extensions;

namespace Soda.Bee.Api.Hubs;

public interface IChatHubClient
{
    Task NotifyMessage(string user, string message);
    Task ReceiveMessage(string user, string message);

    Task ReceiveGroupMessage(string groupName, string message);

    Task CreateGroup(string groupName);
}

public class ChatHub : Hub<IChatHubClient>
{
    private string GetCurrentUser()
    {
        return Context.ConnectionId;
    }
    
    public async Task SendMessage(string user, string message)
    {
        await Clients.Users(user).ReceiveMessage(GetCurrentUser(), message);
    }
    
    /// <summary>
    /// 广播(所有人)
    /// </summary>
    /// <param name="type"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public async Task BroadcastMessage(string message)
    {
        await Clients.All.NotifyMessage(GetCurrentUser(), message);
    }
    /// <summary>
    /// 通知(除发送人外所有人)
    /// </summary>
    /// <param name="type"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public async Task NotifyMessage(string message)
    {
        await Clients.Others.NotifyMessage(GetCurrentUser(), message);
    }
    /// <summary>
    /// 同组内通知
    /// </summary>
    /// <param name="groupName"></param>
    /// <param name="type"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public async Task SendGroupMessage(string groupName,  string message)
    {
        await Clients.OthersInGroup(groupName).ReceiveGroupMessage(groupName, message);
    }
    /// <summary>
    /// 同组内广播
    /// </summary>
    /// <param name="groupName"></param>
    /// <param name="type"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public async Task BroadcastGroupMessage(string groupName, string message)
    {
        await Clients.Groups(groupName).ReceiveGroupMessage(groupName, message);
    }

    /// <summary>
    /// 加入组
    /// </summary>
    /// <param name="groupName"></param>
    /// <returns></returns>
    public async Task AddToGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        await Clients.Groups(groupName).ReceiveGroupMessage(groupName, $"{GetCurrentUser()} 加入组 {groupName}.");
    }
    /// <summary>
    /// 离开组
    /// </summary>
    /// <param name="groupName"></param>
    /// <returns></returns>
    public async Task RemoveFromGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

        await Clients.Groups(groupName).ReceiveGroupMessage(groupName, $"{GetCurrentUser()} 离开组 {groupName}.");
    }

    public async Task CreateGroup(string groupName)
    {
        await AddToGroup(groupName);

        await Clients.All.CreateGroup(groupName);
    }
}

public record Message
{
    public required string Command { get; set; }
    public object Data { get; set; } = new();
}