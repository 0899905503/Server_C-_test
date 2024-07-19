using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly MySqlConnection _conn;

    public AuthController()
    {
        string connectionString = "server=localhost; database=pizza; user=root; password=123456789";
        _conn = new MySqlConnection(connectionString);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLogin login)
    {
        var user = await AuthenticateUserAsync(login.Username, login.Password);

        if (user == null)
            return Unauthorized(new { Message = "Invalid username or password" });

        var token = GenerateJwtToken(user);
        return Ok(new { Token = token, User = user });
    }

    private async Task<User> AuthenticateUserAsync(string username, string password)
    {
        User user = null;
        try
        {
            await _conn.OpenAsync();
            string query = "SELECT * FROM user WHERE username=@username AND password=@password";
            MySqlCommand cmd = new MySqlCommand(query, _conn);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@password", password);
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    user = new User
                    {
                        Id = reader.GetInt32("id"),
                        Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? null : reader.GetString("Name"),
                        Username = reader.GetString("username"),
                        Role = reader.IsDBNull(reader.GetOrdinal("role")) ? null : reader.GetString("role"),

                        Phone_Number = reader.IsDBNull(reader.GetOrdinal("phone_number")) ? null : reader.GetString("phone_number"),
                        Address = reader.IsDBNull(reader.GetOrdinal("address")) ? null : reader.GetString("address"),

                    };
                }
            }
        }
        finally
        {
            await _conn.CloseAsync();
        }

        return user;
    }

    private string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes("pQ3qZw8nC$e9hTmYxA!tFvUdRbGmLrJk");
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {

                new Claim(ClaimTypes.Name, user.Name ?? string.Empty)  ,
                new Claim(ClaimTypes.Name, user.Username ?? string.Empty),
                new Claim(ClaimTypes.Name, user.Password ?? string.Empty),
                          new Claim(ClaimTypes.NameIdentifier, user.Id.ToString() ?? string.Empty)
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    [HttpDelete]
    public async Task<IActionResult> Delete(int id)
    {

        try
        {
            _conn.Open();
            string query = "delete from user where id=@id";
            MySqlCommand cmd = new MySqlCommand(query, _conn);
            cmd.Parameters.AddWithValue("@id", id);

            int result = await cmd.ExecuteNonQueryAsync();
            if (result == 0)
            {
                return NotFound(new { message = "Not found User Id" });
            }
        }
        finally
        {
            await _conn.CloseAsync();
        }
        return Ok(new { Message = "User deleted successfully" });
    }
}

public class UserLogin
{
    public string? Username { get; set; }
    public string? Password { get; set; }
}

