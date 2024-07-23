
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Pizza;
using Pizza.Models;
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
                        Id = reader.GetInt32("Id"),
                        taste = reader.IsDBNull(reader.GetOrdinal("taste")) ? null : reader.GetString("taste"),
                        Price = reader.GetDouble("Price")
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
        [HttpGet("{Id}")]
        public ActionResult<IEnumerable<Taste>> GetTasteById(int Id)
        {
            Taste taste = null;
            try
            {
                _conn.Open();
                string query = "Select*from Type where Id=@Id";
                MySqlCommand cmd = new MySqlCommand(query, _conn);
                cmd.Parameters.AddWithValue("@Id", Id);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    taste = new Taste
                    {
                        Id = reader.GetInt32("Id"),
                        taste = reader.IsDBNull(reader.GetOrdinal("taste")) ? null : reader.GetString("taste"),
                        Price = reader.GetDouble("Price")
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
        [HttpPut("update_Price/{Id}")]
        public IActionResult UpdateTastePrice(int Id, [FromBody] Taste updatedTaste)
        {
            if (updatedTaste == null || updatedTaste.Id != Id)
            {
                return BadRequest();
            }

            try
            {
                _conn.Open();
                string query = "UPDATE Type SET Price = @Price WHERE Id = @Id";
                MySqlCommand cmd = new MySqlCommand(query, _conn);
                cmd.Parameters.AddWithValue("@Price", updatedTaste.Price);
                cmd.Parameters.AddWithValue("@Id", Id);

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
                string query = "INSERT INTO Type (taste, Price) VALUES (@taste, @Price)";
                MySqlCommand cmd = new MySqlCommand(query, _conn);
                cmd.Parameters.AddWithValue("@taste", newTaste.taste);
                cmd.Parameters.AddWithValue("@Price", newTaste.Price);

                cmd.ExecuteNonQuery();
                newTaste.Id = (int)cmd.LastInsertedId;
            }
            finally
            {
                _conn.Close();
            }

            return CreatedAtAction(nameof(GetTasteById), new { Id = newTaste.Id }, newTaste);
        }
    }
}
