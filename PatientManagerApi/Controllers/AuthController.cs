using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PatientManagerApi.Models.MedicUser;
using PatientManagerApi.Service.Interfaces;

namespace PatientManageApi.Controllers;

[ApiController]
[Route("api/[controller]")]
// [AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService  authService)
    {
        _authService = authService;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRequest request)
    {
        if (request.Email == null || request.Password == null)
        {
            return BadRequest("All data must be filled");
        }
        
        var result = await _authService.RegisterUser(request);
        if (!result)
        {
            return BadRequest("User already exists");
        }
        
        return Ok(new { Message = "User registration successful." });
    }
    
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserRequest request)
    {
        if (request.Email == null || request.Password == null)
        {
            return BadRequest("All data must be filled");
        }

        var result = await _authService.LoginUser(request);
        
        if (!result)
        {
            return Unauthorized("Username or password are incorrect.");
        }

        return Ok(new { Message = "Login successful." });
    }
}
