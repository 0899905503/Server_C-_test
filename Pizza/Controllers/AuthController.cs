using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;
using Pizza.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Pizza.Models
{
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

        [HttpPost("google-signin")]
        public async Task<IActionResult> GoogleSignIn([FromBody] TokenRequest tokenRequest)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(tokenRequest.IdToken);
                var user = await GetUserByEmailAsync(payload.Email);

                if (user == null)
                {
                    user = new User
                    {
                        Username = payload.Email,
                        Name = payload.Email,
                        Role = "customer",
                    };

                    await CreateUserAsync(user);
                }

                var token = GenerateJwtToken(user);
                return Ok(new { Token = token, User = user });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Google Sign-In failed", Error = ex.Message });
            }
        }

        private async Task<User> AuthenticateUserAsync(string username, string password)
        {
            User user = null;
            try
            {
                await _conn.OpenAsync();
                string query = "SELECT * FROM users WHERE username=@username AND password=@password";
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

        private async Task<User> GetUserByEmailAsync(string email)
        {
            User user = null;
            try
            {
                await _conn.OpenAsync();
                string query = "SELECT * FROM users WHERE username=@username";
                MySqlCommand cmd = new MySqlCommand(query, _conn);
                cmd.Parameters.AddWithValue("@username", email);
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

        private async Task CreateUserAsync(User user)
        {
            try
            {
                await _conn.OpenAsync();
                string query = "INSERT INTO users (username, name, role) VALUES (@username, @name, @role)";
                MySqlCommand cmd = new MySqlCommand(query, _conn);
                cmd.Parameters.AddWithValue("@username", user.Username);
                cmd.Parameters.AddWithValue("@name", user.Name);
                cmd.Parameters.AddWithValue("@role", user.Role);
                await cmd.ExecuteNonQueryAsync();
                user.Id = (int)cmd.LastInsertedId;
            }
            finally
            {
                await _conn.CloseAsync();
            }
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("pQ3qZw8nC$e9hTmYxA!tFvUdRbGmLrJk");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Name ?? string.Empty),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString() ?? string.Empty),
                    new Claim(ClaimTypes.Email, user.Username ?? string.Empty),
                    new Claim(ClaimTypes.Role, user.Role ?? string.Empty)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        [HttpGet("{Id}")]
        public ActionResult<IEnumerable<Taste>> GetTasteById(int Id)
        {
            User user = null;
            try
            {
                _conn.Open();
                string query = "Select * from users where Id=@Id";
                MySqlCommand cmd = new MySqlCommand(query, _conn);
                cmd.Parameters.AddWithValue("@Id", Id);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    user = new User
                    {
                        Id = reader.GetInt32("Id"),
                        Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? null : reader.GetString("Name"),
                        Role = reader.IsDBNull(reader.GetOrdinal("Role")) ? null : reader.GetString("Role"),
                        Phone_Number = reader.IsDBNull(reader.GetOrdinal("Phone_Number")) ? null : reader.GetString("Phone_Number"),
                        Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? null : reader.GetString("Address"),
                    };
                }
                reader.Close();
            }
            finally
            {
                _conn.Close();
            }
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                _conn.Open();
                string query = "DELETE FROM users WHERE id=@id";
                MySqlCommand cmd = new MySqlCommand(query, _conn);
                cmd.Parameters.AddWithValue("@id", id);

                int result = await cmd.ExecuteNonQueryAsync();
                if (result == 0)
                {
                    return NotFound(new { message = "User not found" });
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

    public class TokenRequest
    {
        public string IdToken { get; set; }
    }
}
