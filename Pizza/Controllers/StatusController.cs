using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

[Route("api/[controller]")]
[ApiController]

public class StatusController : ControllerBase
{
    private readonly MySqlConnection _conn;
    public StatusController()
    {

        string connectionString = "server=localhost; database=pizza; user=root; password=123456789";
        _conn = new MySqlConnection(connectionString);
    }
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly MySqlConnection _conn;

        public OrderController()
        {
            string connectionString = "server=localhost; database=pizza; user=root; password=123456789";
            _conn = new MySqlConnection(connectionString);
        }

        // Endpoint để cập nhật trạng thái đơn hàng
        [HttpPut("{orderId}/status")]
        public IActionResult UpdateOrderStatus(int orderId, [FromBody] string status)
        {
            if (string.IsNullOrEmpty(status))
            {
                return BadRequest(new { Message = "Invalid status." });
            }

            try
            {
                _conn.Open();
                string query = "UPDATE Orders SET Status = @Status WHERE OrderId = @OrderId";
                MySqlCommand cmd = new MySqlCommand(query, _conn);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@OrderId", orderId);
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    return NotFound();
                }

                return Ok(new { Message = "Order status updated successfully." });
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

}