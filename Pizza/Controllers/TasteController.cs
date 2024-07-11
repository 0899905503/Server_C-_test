
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
    }
}
