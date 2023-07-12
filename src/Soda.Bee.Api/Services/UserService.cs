using System.Collections.Concurrent;
using Soda.Bee.Shared;

namespace Soda.Bee.Api.Services;

public interface IUserService
{
    public ConcurrentDictionary<string, User> Users { get; }

    void AddUser(User user);

    void DelUser(string id);

    User? GetUser(string id);

    PagedList<User> GetUsers(QueryParam? param = null);

    void AddToGroup(string userId, string groupId);

    void LeaveGroup(string userId, string groupId);
}

public class UserService : IUserService
{
    private readonly ConcurrentDictionary<string, User> _users = new();

    ConcurrentDictionary<string, User> IUserService.Users => _users;

    public void AddToGroup(string userId, string groupId)
    {
        if (_users.TryGetValue(userId, out var user))
        {
            user.GroupIds.Add(groupId);

            _users[userId] = user;
        }
    }

    public void AddUser(User user)
    {
        _users.TryAdd(user.ConnectionId, user);
    }

    public void DelUser(string id)
    {
        _users.TryRemove(id, out _);
    }

    public User? GetUser(string id)
    {
        _users.TryGetValue(id, out var user);
        return user;
    }

    public PagedList<User> GetUsers(QueryParam? param = null)
    {
        var query = _users.Values as IQueryable<User>;
        if (!string.IsNullOrWhiteSpace(param?.SearchKey))
        {
            query = query?.Where(x => x.Name.Contains(param.SearchKey));
        }

        return PagedList<User>.Create(query!, param?.Page ?? 0, param?.PageSize ?? 10);
    }

    public void LeaveGroup(string userId, string groupId)
    {
        if (_users.TryGetValue(userId, out var user))
        {
            user.GroupIds.Remove(groupId);

            _users[userId] = user;
        }
    }
}