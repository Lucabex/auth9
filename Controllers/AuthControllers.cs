using Microsoft.AspNetCore.Mvc;
using auth9.DTO;
using auth9.Data;
using auth9.Records;
using Microsoft.EntityFrameworkCore;
using auth9.Models;
namespace auth9.Controllers;

[ApiController]
[Route("auth")]
public class AuthControllers : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IHttpClientFactory _factory;

    public AuthControllers(AppDbContext context,IHttpClientFactory factory)
    {
        _context = context;
        _factory = factory;
    }
    [HttpPost("reg")]
    public async Task<IActionResult> RegUser(RegDto dto)
    {
        if (string.IsNullOrEmpty(dto.Name) || string.IsNullOrEmpty(dto.Password))
        {
            return BadRequest("Please add username and Password");
        }
        if(await _context.User.AnyAsync(u=>(u.Name ?? "").ToLower() == dto.Name.ToLower()))
        {
            return BadRequest("User already in use");
        }
        var user = new User
        {
            Name = dto.Name,
            HashedPassowrd = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };
        var response = new RegResp
        {
            Id = user.Id,
            Name = dto.Name
        };
        return Ok(response);
    }
    [HttpPost("log")]
    public async Task<IActionResult> LogUser(LogDto dto)
    {
        if(string.IsNullOrEmpty(dto.Name) || string.IsNullOrEmpty(dto.Password))
        {
            return BadRequest("Please add username or Password");
        }
        var user =await _context.User.FirstOrDefaultAsync(u=>(u.Name ?? "").ToLower() == dto.Name.ToLower());

        if(user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.HashedPassowrd))
        {
            return BadRequest("Invalid username or Password");

        }
        var response = new LogResp
        {
            Id = user.Id,
            Name = user.Name
        };
        return Ok(response);

    }
    [HttpGet("puzzle")]
    public async Task<IActionResult> GetPuzzle()
    {
        var client = _factory.CreateClient();
        var url = "https://lichess.org/api/puzzle/daily";
        var response = await client.GetFromJsonAsync<PuzzleDaily>(url);
        if(response?.Solution==null || response?.Fen== null)
        {
            return StatusCode(503,"Service not availabe try again later");
        }
        return Ok(response);

    }
}