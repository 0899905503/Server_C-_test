using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

[Route("api/[controller]")]

public class RoleController : ControllerBase
{
    private readonly MySqlConnection _conn;
    public RoleController()
    {

        string connectionString = "server=localhost; database=pizza; user=root; password=123456789";
        _conn = new MySqlConnection(connectionString);
    }

}