using Microsoft.EntityFrameworkCore;
using Pizza.Models;


public class PizzaDbContext : DbContext
{
    public PizzaDbContext(DbContextOptions<PizzaDbContext> options)
        : base(options)
    {
    }


    public DbSet<User> Users { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<Taste> Tastes { get; set; }
    public DbSet<Flavor> Flavors { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // modelBuilder.Entity<User>()
        //        .HasData(
        //            new User { Id = 1, Name = "Admin", Username = "admin", Password = "admin", Role = "Admin", Phone_Number = "123-456-7890", Address = "123 Main St" },
        //            new User { Id = 2, Name = "User", Username = "user", Password = "user", Role = "User", Phone_Number = "987-654-3210", Address = "456 Elm St" }
        //        );

        // modelBuilder.Entity<Taste>().HasData(
        //            new Taste { Id = 1, taste = "Tomato", Price = 12 },
        //            new Taste { Id = 2, taste = "Mango", Price = 14 },
        //            new Taste { Id = 3, taste = "Chilly", Price = 16 },
        //            new Taste { Id = 4, taste = "Popcorn", Price = 18 }
        //        );

    }
}
public interface ISeedData
{
    void Initialize(PizzaDbContext context);
}

// public class SeedData : ISeedData
// {
//     public void Initialize(PizzaDbContext context)
//     {
//         // Kiểm tra và chèn dữ liệu nếu chưa có
//         if (!context.Users.Any())
//         {
//             context.Users.AddRange(
//                 new User { Id = 1, Name = "Admin", Username = "admin", Password = "admin", Role = "Admin", Phone_Number = "123-456-7890", Address = "123 Main St" },
//                 new User { Id = 2, Name = "User", Username = "user", Password = "user", Role = "User", Phone_Number = "987-654-3210", Address = "456 Elm St" }
//             );
//             context.SaveChanges();
//         }

//         if (!context.Tastes.Any())
//         {
//             context.Tastes.AddRange(
//                 new Taste { Id = 1, taste = "Tomato", Price = 12 },
//                 new Taste { Id = 2, taste = "Mango", Price = 14 },
//                 new Taste { Id = 3, taste = "Chilly", Price = 16 },
//                 new Taste { Id = 4, taste = "Popcorn", Price = 18 }
//             );
//             context.SaveChanges();
//         }
//     }
// }
