using System.Security.Claims;
using CalisthenicsApi.Models;
using CalisthenicsApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CalisthenicsApi.Controllers;

[ApiController]
[Route("api/[controller]")]

public class WorkoutController : ControllerBase{

    private readonly WorkoutService _workoutService;
    private readonly UserService _userService;
    public WorkoutController(WorkoutService workoutService, UserService userService){
        _workoutService = workoutService;
        _userService = userService;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateWorkout(Workout workout){
        var token = HttpContext.Request.Headers["Authorization"];
        var id = User.FindFirst("id").Value;
        workout.User = id;
        var workoutId = await _workoutService.CreateAsync(workout);
        User user =  await _userService.GetByIdAsync(id);
        user.Workouts.Add(workoutId);
        await _userService.UpdateAsync(id,user);
        return Ok(workoutId);
    }
    
}