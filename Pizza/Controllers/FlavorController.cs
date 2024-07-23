using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System.Collections.Generic;
using System.Linq;

[Route("api/flavors")]
[ApiController]

public class FlavorsController : ControllerBase
{

    private readonly MySqlConnection _conn;

    public FlavorsController()
    {
        string connectionString = "server=localhost; database=pizza; user=root; password=123456789";
        _conn = new MySqlConnection(connectionString);
    }
    // GET: api/flavors
    [HttpGet]
    public ActionResult<IEnumerable<Flavor>> GetFlavors()
    {
        List<Flavor> flavors = new List<Flavor>();
        try
        {
            _conn.Open();

            string query = "select * from flavor";
            MySqlCommand cmd = new MySqlCommand(query, _conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Flavor flavor = new Flavor
                {
                    Id = reader.GetInt32("id"),
                    flavor = reader.IsDBNull(reader.GetOrdinal("flavor")) ? null : reader.GetString("flavor"),
                    Price = reader.GetDouble("Price")
                };

                flavors.Add(flavor);
            }

            reader.Close();
        }
        finally
        {

        }
        return Ok(flavors);
    }

    [HttpGet("{id}")]
    public ActionResult<IEnumerable<Flavor>> GetFlavorsById(int Id)
    {
        Flavor flavor = null;
        try
        {
            _conn.Open();
            string query = "select *from flavor whereId=@id";
            MySqlCommand cmd = new MySqlCommand(query, _conn);
            cmd.Parameters.AddWithValue("@id", Id);
            MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                flavor = new Flavor
                {
                    Id = reader.GetInt32("id"),
                    flavor = reader.IsDBNull(reader.GetOrdinal("flavor")) ? null : reader.GetString("flavor"),
                    Price = reader.GetDouble("Price")
                };
            }
            reader.Close();
        }
        finally { _conn.Close(); }
        if (flavor == null)
        {
            return NotFound();
        }

        return Ok(flavor);
    }
}
