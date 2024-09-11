using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CalisthenicsApi.Models;
using CalisthenicsApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;

namespace CalisthenicsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase{
    private readonly UserService _userService;
    private readonly IConfiguration _configuration; 
    
    public AuthController(UserService userService, IConfiguration configuration) {
        _userService = userService;
        _configuration = configuration;
        }
    
    [HttpPost("create")]
    public async Task<IActionResult> PostUser(User user){
        User userLookUp = await _userService.GetByEmailAsync(user.Email);
        if (userLookUp != null) return BadRequest("Email already exists");
        user.Password = PasswordService.HashPassword(user.Password);
        string id = await _userService.CreateAsync(user);
        User createdUser = await _userService.GetByIdAsync(id);
        string token = GenerateToken(createdUser);
        var cookie = new CookieOptions(){
            Expires = DateTimeOffset.Now.AddDays(48),
            Path = "/"
        };
        Response.Cookies.Append("token", token, cookie);
        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginUser(LoginUser login){
        User user = await _userService.GetByEmailAsync(login.Email);
        if (user == null) return BadRequest("User does not exist");
        Boolean passwordCorrect = PasswordService.VerifyPassword(login.Password, user.Password);
        if (!passwordCorrect) return BadRequest("Password is Incorrect");
        string token = GenerateToken(user);
        var cookie = new CookieOptions(){
            Expires = DateTimeOffset.Now.AddDays(48),
            Path = "/"
        };
        Response.Cookies.Append("token", token, cookie);
        return Ok();
        
    }



    private string GenerateToken(User user){
        var handler = new JwtSecurityTokenHandler();

        //Here we are creating the creating the credentials to sign the token. Key is converted to bytes because algorithm needs it in that form to work
        var privateKey = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(privateKey), 
            SecurityAlgorithms.HmacSha256
        );

        //This add claims to the token, it is automatically added to the payload
        var ci = new ClaimsIdentity();
        ci.AddClaim(new Claim("id", user.Id));
        ci.AddClaim(new Claim(ClaimTypes.Email, user.Email));
        ci.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
        ci.AddClaim(new Claim(ClaimTypes.GivenName, user.FirstName));

        //This includes the information within the token
        var tokenDescriptor = new SecurityTokenDescriptor{
            SigningCredentials = credentials,
            Expires = DateTime.UtcNow.AddDays(48),
            Subject = ci
        };
        
        //Creating The Token
        var token = handler.CreateToken(tokenDescriptor);

        //Sending the token as string
        return handler.WriteToken(token);
    }
}