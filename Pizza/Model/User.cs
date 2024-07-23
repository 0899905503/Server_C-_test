using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Pizza;


public class PizzaContext : DbContext
{

    public DbSet<User> Users { get; set; }
}
public class TokenResponse
{
    public string Token { get; set; }
    public User User { get; set; }
}

public class User
{

    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
    public string Phone_Number { get; set; }
    public string Address { get; set; }
}
