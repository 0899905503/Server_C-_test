
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Pizza;
using System.Collections.Generic;

namespace YourNamespace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class TasteController : ControllerBase
    {
        private readonly MySqlConnection _conn;

        public TasteController()
        {
            string connectionString = "server=localhost; database=pizza; user=root; password=123456789";
            _conn = new MySqlConnection(connectionString);
        }

        [HttpGet]
        public ActionResult<IEnumerable<TasteController>> Get()
        {
            List<Taste> tastes = new List<Taste>();

            try
            {
                _conn.Open();

                string query = "SELECT * FROM Type";
                MySqlCommand cmd = new MySqlCommand(query, _conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Taste taste = new Taste
                    {
                        id = reader.GetInt32("id"),
                        taste = reader.IsDBNull(reader.GetOrdinal("taste")) ? null : reader.GetString("taste"),
                        price = reader.GetDouble("price")
                    };

                    tastes.Add(taste);
                }

                reader.Close();
            }
            finally
            {
                _conn.Close();
            }

            return Ok(tastes);
        }
        [HttpGet("{id}")]
        public ActionResult<IEnumerable<Taste>> GetTasteById(int id)
        {
            Taste taste = null;
            try
            {
                _conn.Open();
                string query = "Select*from Type where id=@id";
                MySqlCommand cmd = new MySqlCommand(query, _conn);
                cmd.Parameters.AddWithValue("@id", id);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    taste = new Taste
                    {
                        id = reader.GetInt32("id"),
                        taste = reader.IsDBNull(reader.GetOrdinal("taste")) ? null : reader.GetString("taste"),
                        price = reader.GetDouble("price")
                    };
                }
                reader.Close();
            }
            finally { _conn.Close(); }
            if (taste == null)
            {
                return NotFound();
            }

            return Ok(taste);

        }
        [HttpPut("update_price/{id}")]
        public IActionResult UpdateTastePrice(int id, [FromBody] Taste updatedTaste)
        {
            if (updatedTaste == null || updatedTaste.id != id)
            {
                return BadRequest();
            }

            try
            {
                _conn.Open();
                string query = "UPDATE Type SET price = @price WHERE id = @id";
                MySqlCommand cmd = new MySqlCommand(query, _conn);
                cmd.Parameters.AddWithValue("@price", updatedTaste.price);
                cmd.Parameters.AddWithValue("@id", id);

                int result = cmd.ExecuteNonQuery();
                if (result == 0)
                {
                    return NotFound();
                }
            }
            finally
            {
                _conn.Close();
            }

            return NoContent();
        }
        [HttpPost("create_taste")]
        public ActionResult<Taste> CreateTaste([FromBody] Taste newTaste)
        {
            if (newTaste == null)
            {
                return BadRequest();
            }

            try
            {
                _conn.Open();
                string query = "INSERT INTO Type (taste, price) VALUES (@taste, @price)";
                MySqlCommand cmd = new MySqlCommand(query, _conn);
                cmd.Parameters.AddWithValue("@taste", newTaste.taste);
                cmd.Parameters.AddWithValue("@price", newTaste.price);

                cmd.ExecuteNonQuery();
                newTaste.id = (int)cmd.LastInsertedId;
            }
            finally
            {
                _conn.Close();
            }

            return CreatedAtAction(nameof(GetTasteById), new { id = newTaste.id }, newTaste);
        }
    }
}
