using Microsoft.AspNetCore.Mvc;
using Soda.Bee.Api.Services;
using Soda.Bee.Shared;

namespace Soda.Bee.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ApiControllerBase : ControllerBase
{
}

public class GroupController : ApiControllerBase
{
    private readonly IGroupService _groupService;

    public GroupController(IGroupService groupService)
    {
        _groupService = groupService;
    }

    [HttpGet]
    public IActionResult GetGroups([FromQuery] QueryParam param)
    {
        var res = _groupService.GetGroups(param);

        return Ok(res);
    }

    [HttpGet]
    [Route("{id}")]
    public IActionResult GetGroup(string id)
    {
        var res = _groupService.GetGroup(id);

        return Ok(res);
    }

    [HttpPost]
    public IActionResult AddGroup([FromBody] Group group)
    {
        if (string.IsNullOrWhiteSpace(group.Name))
        {
            throw new ArgumentNullException(nameof(group));
        }
        _groupService.AddGroup(group);

        return NoContent();
    }

    [HttpDelete]
    [Route("{id}")]
    public IActionResult DeleteGroup(string id)
    {
        _groupService.DelGroup(id);
        return NoContent();
    }

    [HttpGet]
    [Route("users")]
    public IActionResult GetGroupUsers([FromQuery] QueryParam param)
    {
        var res = _groupService.GetGroupUsers(param);
        return Ok(res);
    }
}