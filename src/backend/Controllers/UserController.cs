using CalisthenicsApi.Models;
using CalisthenicsApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CalisthenicsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(UserService userService,ILogger<UserController> logger ){
        _userService = userService;
        _logger = logger;
    }
    [Authorize]
    [HttpGet]
    public async Task<List<User>> GetUsers() {
       _logger.LogInformation("Cookies: {Cookies}", Request.Cookies);
       return await _userService.GetAsync();
    }
    [HttpGet("{id:length(24)}")]
    public async Task<IActionResult> GetUserAsync(string id) {
        User output = await _userService.GetByIdAsync(id);
        return output is null ? BadRequest("Id was not found") : Ok(output);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteUser(string id) {
        if(id.Length != 24) return BadRequest("Id length must be 24");
        User user = await _userService.GetByIdAsync(id);
        if (user == null) return NotFound();
        await _userService.RemoveAsync(id);
        return Ok();
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> PutUser(User updatedUser, string id){
        User user = await _userService.GetByIdAsync(id);
        if (user is null) return NotFound();
        await _userService.UpdateAsync(id, user);
        return Ok();
    }
}