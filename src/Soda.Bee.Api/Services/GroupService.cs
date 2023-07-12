using System.Collections.Concurrent;
using Soda.Bee.Shared;

namespace Soda.Bee.Api.Services;

public interface IGroupService
{
    public ConcurrentDictionary<string, Group> Groups { get; }

    PagedList<Group> GetGroups(QueryParam? param = null);

    Group? GetGroup(string id);

    void AddGroup(Group group);

    void DelGroup(string id);

    PagedList<User> GetGroupUsers(QueryParam? param = null);

    PagedList<Group> GetUserGroups(QueryParam? param = null);
}

public class GroupService : IGroupService
{
    private readonly IUserService _userService;

    private readonly ConcurrentDictionary<string, Group> _groups = new();

    public GroupService(IUserService userService)
    {
        _userService = userService;
    }

    ConcurrentDictionary<string, Group> IGroupService.Groups => _groups;

    public void AddGroup(Group group)
    {
        _groups.TryAdd(group.Id, group);
    }

    public void DelGroup(string id)
    {
        _groups.TryRemove(id, out var _);
    }

    public Group? GetGroup(string id)
    {
        _groups.TryGetValue(id, out var group);
        return group;
    }

    public PagedList<Group> GetGroups(QueryParam? param = null)
    {
        var query = _groups.Values as IQueryable<Group>;
        if (!string.IsNullOrWhiteSpace(param?.SearchKey))
        {
            query = query?.Where(x => x.Name.Contains(param.SearchKey) || x.Id.Contains(param.SearchKey) || x.Description.Contains(param.SearchKey));
        }

        return PagedList<Group>.Create(query!, param?.Page ?? 0, param?.PageSize ?? 10);
    }

    public PagedList<User> GetGroupUsers(QueryParam? param = null)
    {
        var query = _userService.Users.Values as IQueryable<User>;
        if (!string.IsNullOrWhiteSpace(param?.SearchKey))
        {
            query = query?.Where(x => x.GroupIds.Contains(param.SearchKey));
        }

        return PagedList<User>.Create(query!, param?.Page ?? 0, param?.PageSize ?? 10);
    }

    public PagedList<Group> GetUserGroups(QueryParam? param = null)
    {
        var query = _groups.Values as IQueryable<Group>;

        var groupIds = _userService.GetUser(param?.SearchKey ?? "")?.GroupIds ?? Enumerable.Empty<string>();

        query = query?.Where(x => groupIds.Contains(x.Id));

        return PagedList<Group>.Create(query!, param?.Page ?? 0, param?.PageSize ?? 10);
    }
}