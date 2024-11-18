using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Models;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace UserService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserContext _context;

        public UsersController(UserContext context)
        {
            _context = context;
        }

        // Obtener todos los usuarios (solo para administradores)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // Registrar un nuevo usuario
        [HttpPost("register")]
        public async Task<ActionResult<User>> RegisterUser(User user)
        {
            // Aquí podríamos agregar el hashing de contraseñas (se implementará más adelante)
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        // Autenticar un usuario (login)
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginRequest.Email);
            if (user == null || !VerifyPassword(loginRequest.Password, user.PasswordHash))
            {
                return Unauthorized("Credenciales inválidas.");
            }

            var token = GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<ActionResult<User>> GetUserProfile()
        {



            // Obtener el email del usuario desde las claims del token JWT
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            Console.WriteLine($"Email extraído del token: {email}");

            if (email == null) return Unauthorized("Usuario no autorizado.");

            // Buscar al usuario en la base de datos
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return NotFound("Usuario no encontrado.");

            // Construir la respuesta excluyendo datos sensibles
            var userProfile = new
            {
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email,
                user.Role,
                user.CreatedAt
            };

            return Ok(userProfile);
        }

        // Obtener un usuario por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            return user;
        }

        // Actualizar un usuario
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, User user)
        {
            if (id != user.Id) return BadRequest();

            _context.Entry(user).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Users.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        // Eliminar un usuario
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Métodos auxiliares para hashing y JWT se agregarán más adelante
  

private bool VerifyPassword(string password, string storedHash)
{
    // Aquí asumimos que el hash almacenado incluye el salt y el hash juntos, separados por ':'
    var parts = storedHash.Split(':');
    if (parts.Length != 2) return false;

    var salt = Convert.FromBase64String(parts[0]);
    var hash = Convert.FromBase64String(parts[1]);

    // Generar el hash de la contraseña proporcionada con el mismo salt
    var hashToVerify = KeyDerivation.Pbkdf2(
        password: password,
        salt: salt,
        prf: KeyDerivationPrf.HMACSHA256,
        iterationCount: 10000,
        numBytesRequested: 32);

    // Comparar los hashes
    return CryptographicOperations.FixedTimeEquals(hash, hashToVerify);
}

private string GenerateJwtToken(User user)
{
    var jwtKey = "9qJ&3#7v!2tVx^D*kPp@Fm5UzR#cL$Q8";
    var jwtIssuer = "your_issuer";
    var jwtAudience = "your_audience";

    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new[]
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Email),
        new Claim(ClaimTypes.Email, user.Email), // Asegúrate de incluir esta claim
        new Claim(ClaimTypes.Name, user.FirstName),
        new Claim(ClaimTypes.Role, user.Role),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

    var token = new JwtSecurityToken(
        issuer: jwtIssuer,
        audience: jwtAudience,
        claims: claims,
        expires: DateTime.UtcNow.AddHours(1),
        signingCredentials: credentials);

    return new JwtSecurityTokenHandler().WriteToken(token);
}

}

}



