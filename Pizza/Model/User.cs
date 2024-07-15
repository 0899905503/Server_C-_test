using Microsoft.EntityFrameworkCore;

public class PizzaContext : DbContext
{

    public DbSet<User> Users { get; set; }
}

public class User
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}
