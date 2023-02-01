using dotnet_rpg.Data;
using dotnet_rpg.Dtos.User;
using dotnet_rpg.Models;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthRepository _authRepo;

    public AuthController(IAuthRepository authRepo)
    {
        _authRepo = authRepo;
    }
    
    [HttpPost("register")]
    public async Task<ActionResult<ServiceResponse<int>>> Register(UserRegisterDto request)
    {
        ServiceResponse<int> response = await _authRepo.Register(
            new User { Username = request.Username }, request.Password
        );
        
        if (!response.Success)
        {
            return BadRequest(response);
        }
        
        return Ok(response);
    }
    
    [HttpPost("login")]
    public async Task<ActionResult<ServiceResponse<string>>> Login(UserLoginDto request)
    {
        ServiceResponse<string> response = await _authRepo.Login(
            request.Username, request.Password
        );
        
        if (!response.Success)
        {
            return BadRequest(response);
        }
        
        return Ok(response);
    }
}