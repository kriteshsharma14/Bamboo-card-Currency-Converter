using Bamboo_card_currency_convertor.Models;
using Bamboo_card_currency_convertor.Services.Interface;
using Bamboo_card_currency_convertor.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Bamboo_card_currency_convertor.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;

        // Fake user store for demo purposes
        private readonly List<User> _users = new()
            {
                new User { Username = "manager", Role = Roles.Manager },
                new User { Username = "user", Role = Roles.User },
                 new User { Username = "admin", Role = Roles.Admin }
            };

        public AuthController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserCredential credential)
        {
            var user = _users.FirstOrDefault(x =>
                x.Username == credential.Username && credential.Password == "password"); // simplified

            if (user == null)
                return Unauthorized("Invalid credentials");

            var token = _tokenService.GenerateToken(user);
            return Ok(new { Token = token });
        }
    }
}
