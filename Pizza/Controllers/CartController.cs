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

    [HttpPost]
    public IActionResult SaveCart([FromBody] Cart cart)
    {
        if (cart == null)
        {
            return BadRequest(new { Message = "Invalid cart data." });
        }

        try
        {
            _conn.Open();
            string query = "INSERT INTO Cart (Pizza, Flavor, price) VALUES (@Taste, @Flavor, @price)";
            using (MySqlCommand cmd = new MySqlCommand(query, _conn))
            {
                cmd.Parameters.AddWithValue("@Taste", cart.Taste);
                cmd.Parameters.AddWithValue("@Flavor", cart.Flavor);
                cmd.Parameters.AddWithValue("@price", cart.price);

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
            string query = "SELECT * FROM Cart";
            using (MySqlCommand cmd = new MySqlCommand(query, _conn))
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Cart cart = new Cart
                        {
                            Id = reader.GetInt32("id"),
                            Taste = reader.IsDBNull(reader.GetOrdinal("Pizza")) ? null : reader.GetString("Pizza"),
                            Flavor = reader.IsDBNull(reader.GetOrdinal("Flavor")) ? null : reader.GetString("Flavor"),
                            price = reader.GetDouble("price"),
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
    [HttpDelete("clear")]
    public async Task<IActionResult> ClearCartAsync()
    {
        try
        {
            if (_conn.State != System.Data.ConnectionState.Open)
                await _conn.OpenAsync();


            string query = "delete from cart";
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
            string query = "delete from cart where id=@id";
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
}


