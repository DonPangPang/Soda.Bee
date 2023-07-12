using Microsoft.AspNetCore.Mvc;
using Soda.Bee.Api.Services;
using Soda.Bee.Shared;

namespace Soda.Bee.Api.Controllers;

public class UserController : ApiControllerBase
{
    private readonly IUserService _userService;
    private readonly IGroupService _groupService;

    public UserController(IUserService userService, IGroupService groupService)
    {
        _groupService = groupService;
        _userService = userService;
    }

    [HttpGet]
    public IActionResult GetUsers([FromQuery] QueryParam param)
    {
        var res = _userService.GetUsers(param);

        return Ok(res);
    }

    [HttpGet]
    [Route("{id}")]
    public IActionResult GetUser(string id)
    {
        var res = _userService.GetUser(id);

        return Ok(res);
    }

    [HttpPost]
    public IActionResult AddUser([FromBody] User User)
    {
        if (string.IsNullOrWhiteSpace(User.Name))
        {
            throw new ArgumentNullException(nameof(User));
        }
        _userService.AddUser(User);

        return NoContent();
    }

    [HttpDelete]
    [Route("{id}")]
    public IActionResult DeleteUser(string id)
    {
        _userService.DelUser(id);
        return NoContent();
    }

    [HttpGet]
    [Route("groups")]
    public IActionResult GetUserUsers([FromQuery] QueryParam param)
    {
        var res = _groupService.GetUserGroups(param);
        return Ok(res);
    }
}