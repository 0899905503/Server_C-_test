using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

[Route("api/[controller]")]
[ApiController]

public class GuestCartController : ControllerBase
{
    private readonly MySqlConnection _conn;

    public GuestCartController()
    {
        string connectionString = "server=localhost; database=pizza; user=root; password=123456789";
        _conn = new MySqlConnection(connectionString);
    }
    [HttpPost("saveGuestCart")]
    public IActionResult SaveGuestCart([FromBody] GuestCart cart)
    {
        if (cart == null)
        {
            return BadRequest(new { Message = "Invalid cart data." });
        }

        try
        {
            _conn.Open();
            string query = "INSERT INTO guestcarts (Taste, Flavor, Price,Status,Name,Address,Phone_Number,GuestId) VALUES (@Taste, @Flavor, @Price,@Status, @Name,@Address,@Phone_Number,@GuestId)";
            using (MySqlCommand cmd = new MySqlCommand(query, _conn))
            {
                cmd.Parameters.AddWithValue("@Taste", cart.Taste);
                cmd.Parameters.AddWithValue("@Flavor", cart.Flavor);
                cmd.Parameters.AddWithValue("@Price", cart.Price);
                cmd.Parameters.AddWithValue("@Name", cart.Name);
                cmd.Parameters.AddWithValue("@Address", cart.Address);
                cmd.Parameters.AddWithValue("@Phone_Number", cart.Phone_Number);
                cmd.Parameters.AddWithValue("@Status", "Waiting");
                cmd.Parameters.AddWithValue("@GuestId", cart.GuestId);
                cmd.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An error occurred while saving the GuestCart.", Error = ex.Message });
        }
        finally
        {
            _conn.Close();
        }

        return Ok(new { Message = "GuestCart saved successfully!" });
    }

    [HttpGet]

    public IActionResult GetGuestCart()
    {
        List<GuestCart> carts = new List<GuestCart>();
        try
        {
            _conn.Open();
            string query = "SELECT * FROM guestcarts";
            using (MySqlCommand cmd = new MySqlCommand(query, _conn))
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        GuestCart cart = new GuestCart
                        {
                            Id = reader.GetInt32("id"),
                            Taste = reader.IsDBNull(reader.GetOrdinal("Taste")) ? null : reader.GetString("Taste"),
                            Flavor = reader.IsDBNull(reader.GetOrdinal("Flavor")) ? null : reader.GetString("Flavor"),
                            Price = reader.GetDouble("Price"),
                            Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? null : reader.GetString("Status"),
                            Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? null : reader.GetString("Name"),
                            Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? null : reader.GetString("Address"),
                            Phone_Number = reader.IsDBNull(reader.GetOrdinal("Phone_Number")) ? null : reader.GetString("Phone_Number"),
                            GuestId = reader.IsDBNull(reader.GetOrdinal("GuestId")) ? null : reader.GetString("GuestId"),
                        };

                        carts.Add(cart);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An error occurred while retrieving carts.", Error = ex.Message });
        }
        finally
        {
            _conn.Close();
        }

        return Ok(carts);
    }
    [HttpGet("{guestId}")]
    public ActionResult<IEnumerable<Cart>> GetTasteById(string GuestId)
    {
        var guestcartItems = new List<GuestCart>();

        try
        {
            _conn.Open();
            string query = "SELECT * FROM guestcarts WHERE GuestId = @GuestId";
            using (var cmd = new MySqlCommand(query, _conn))
            {
                cmd.Parameters.AddWithValue("@GuestId", GuestId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var guestcartItem = new GuestCart
                        {
                            Id = reader.GetInt32("Id"),
                            Taste = reader.IsDBNull(reader.GetOrdinal("Taste")) ? null : reader.GetString("Taste"),
                            Flavor = reader.IsDBNull(reader.GetOrdinal("Flavor")) ? null : reader.GetString("Flavor"),
                            Price = reader.GetDouble("Price"),
                            Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? null : reader.GetString("Status"),
                            Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? null : reader.GetString("Name"),
                            Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? null : reader.GetString("Address"),
                            Phone_Number = reader.IsDBNull(reader.GetOrdinal("Phone_Number")) ? null : reader.GetString("Phone_Number"),
                            GuestId = reader.IsDBNull(reader.GetOrdinal("GuestId")) ? null : reader.GetString("GuestId"),
                        };

                        guestcartItems.Add(guestcartItem);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Log exception if needed
            return StatusCode(500, new { Message = "An error occurred while retrieving cart items.", Error = ex.Message });
        }
        finally
        {
            _conn.Close();
        }

        if (guestcartItems.Count == 0)
        {
            return NotFound(new { Message = "No items found for the specified user." });
        }

        return Ok(guestcartItems);
    }
}