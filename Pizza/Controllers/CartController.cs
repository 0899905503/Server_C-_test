using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Pizza;
using System;
using System.Collections.Generic;

[Route("api/[controller]")]
[ApiController]

public class CartController : ControllerBase
{
    private readonly MySqlConnection _conn;

    public CartController()
    {
        string connectionString = "server=localhost; database=pizza; user=root; password=123456789";
        _conn = new MySqlConnection(connectionString);
    }

    [HttpPost("save")]
    public IActionResult SaveCart([FromBody] Cart cart)
    {
        if (cart == null)
        {
            return BadRequest(new { Message = "Invalid cart data." });
        }

        try
        {
            _conn.Open();
            string query = "INSERT INTO carts (Taste, Flavor, Price,Status,UserId) VALUES (@Taste, @Flavor, @Price,@Status,@UserId)";
            using (MySqlCommand cmd = new MySqlCommand(query, _conn))
            {
                cmd.Parameters.AddWithValue("@Taste", cart.Taste);
                cmd.Parameters.AddWithValue("@Flavor", cart.Flavor);
                cmd.Parameters.AddWithValue("@Price", cart.Price);
                cmd.Parameters.AddWithValue("@stats", cart.Status);
                cmd.Parameters.AddWithValue("@Status", "Waiting");
                cmd.Parameters.AddWithValue("@UserId", cart.UserId);
                cmd.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An error occurred while saving the cart.", Error = ex.Message });
        }
        finally
        {
            _conn.Close();
        }

        return Ok(new { Message = "Cart saved successfully!" });
    }

    [HttpGet]

    public IActionResult GetCart()
    {
        List<Cart> carts = new List<Cart>();
        try
        {
            _conn.Open();
            string query = "SELECT * FROM carts";
            using (MySqlCommand cmd = new MySqlCommand(query, _conn))
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Cart cart = new Cart
                        {
                            Id = reader.GetInt32("id"),
                            Taste = reader.IsDBNull(reader.GetOrdinal("Taste")) ? null : reader.GetString("Taste"),
                            Flavor = reader.IsDBNull(reader.GetOrdinal("Flavor")) ? null : reader.GetString("Flavor"),
                            Price = reader.GetDouble("price"),
                            Status = reader.IsDBNull(reader.GetOrdinal("status")) ? null : reader.GetString("status"),
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
    [HttpGet("{userid}")]
    public ActionResult<IEnumerable<Cart>> GetTasteById(int userid)
    {
        var cartItems = new List<Cart>();

        try
        {
            _conn.Open();
            string query = "SELECT * FROM carts WHERE userid = @userid";
            using (var cmd = new MySqlCommand(query, _conn))
            {
                cmd.Parameters.AddWithValue("@userid", userid);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var cartItem = new Cart
                        {
                            Id = reader.GetInt32("Id"),
                            Taste = reader.IsDBNull(reader.GetOrdinal("Taste")) ? null : reader.GetString("Taste"),
                            Flavor = reader.IsDBNull(reader.GetOrdinal("Flavor")) ? null : reader.GetString("Flavor"),
                            Price = reader.GetDouble("Price"),
                            Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? null : reader.GetString("Status"),
                        };

                        cartItems.Add(cartItem);
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

        if (cartItems.Count == 0)
        {
            return NotFound(new { Message = "No items found for the specified user." });
        }

        return Ok(cartItems);
    }

    [HttpDelete("clear")]
    public async Task<IActionResult> ClearCartAsync()
    {
        try
        {
            if (_conn.State != System.Data.ConnectionState.Open)
                await _conn.OpenAsync();


            string query = "delete from carts";
            MySqlCommand cmd = new MySqlCommand(query, _conn);

            int result = await cmd.ExecuteNonQueryAsync();
            if (result == 0)
            {
                return NotFound(new { message = "No data found to delete" });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error deleting data: {ex.Message}");
        }
        finally
        {
            await _conn.CloseAsync();
        }
        return Ok(new { message = "Cart deleted successfully" });
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteItem(int id)
    {

        try
        {
            _conn.Open();
            string query = "delete from carts where id=@id";
            MySqlCommand cmd = new MySqlCommand(query, _conn);
            cmd.Parameters.AddWithValue("@id", id);

            int result = await cmd.ExecuteNonQueryAsync();
            if (result == 0)
            {
                return NotFound(new { message = "Not found Item Id" });
            }
        }
        finally
        {
            await _conn.CloseAsync();
        }
        return Ok(new { Message = "Item deleted successfully" });
    }



    // Endpoint để cập nhật trạng thái đơn hàng
    [HttpPut("{id}/status")]
    public IActionResult UpdateOrderStatus(int id, [FromBody] string status)
    {
        if (string.IsNullOrEmpty(status))
        {
            return BadRequest(new { Message = "Invalid status." });
        }

        try
        {
            _conn.Open();
            string query = "UPDATE carts SET Status = @Status WHERE id = @Id";
            MySqlCommand cmd = new MySqlCommand(query, _conn);
            cmd.Parameters.AddWithValue("@Status", status);
            cmd.Parameters.AddWithValue("@Id", id);
            int rowsAffected = cmd.ExecuteNonQuery();

            if (rowsAffected == 0)
            {
                return NotFound();
            }

            return Ok(new { Message = "Cart status updated successfully." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An error occurred while updating order status.", Error = ex.Message });
        }
        finally
        {
            _conn.Close();
        }

    }

}

