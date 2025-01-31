using Hotel_Management.Data;
using Hotel_Management.Models;
using Hotel_Management.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly HMDbContext context;
        private readonly ITokenRepository tokenRepository;

        public AuthController(HMDbContext context, ITokenRepository tokenRepository)
        {
            this.context = context;
            this.tokenRepository = tokenRepository;
        }

        [HttpPost]
        [Route("Register")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            if (await context.Users.AnyAsync(u => u.Email == registerRequestDto.Email))
                return BadRequest(new { message = "Email is already registered." });

            if (registerRequestDto.RoleId == 1001 || registerRequestDto.RoleId == 1002)
            {
                return BadRequest(new { message = "This role cannot be created" });
            }

            // Validate password complexity
            if (string.IsNullOrWhiteSpace(registerRequestDto.PasswordHash) || registerRequestDto.PasswordHash.Length < 8)
            {
                return BadRequest(new { message = "Password must be at least 8 characters long." });
            }

            // Hash the password
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerRequestDto.PasswordHash);


            var user = new User
            {
                UserName = registerRequestDto.UserName,
                Email = registerRequestDto.Email,
                PasswordHash = hashedPassword,
                RoleId = registerRequestDto.RoleId,
                CreatedDate = DateTime.UtcNow
            };

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
            return Ok(new { message = "User Created Successfully." });
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            // Fetch user by email
            var user = await context.Users.Include(u => u.Role)
                                           .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user != null)
            {
                var isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.PasswordHash, user.PasswordHash);

                if (isPasswordValid)
                {
                    //get the user's role
                    var role = user.Role?.RoleName;

                    if (role != null)
                    {
                        //Create token
                        var token = tokenRepository.CreateJwtToken(user, role);

                        var response = new LoginResponseDto()
                        {
                            Email = loginDto.Email,
                            Roles = new List<string> { role },
                            Token = token
                        };
                        return Ok(response);
                    }
                }
            }

            return BadRequest(new { message = "Username or password incorrect" });
        }

    }
}