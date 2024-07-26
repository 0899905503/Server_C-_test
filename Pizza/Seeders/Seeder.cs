using Pizza.Migrations;

internal class DbInitializer
{
    internal static void Initialize(PizzaDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));
        dbContext.Database.EnsureCreated();
        if (dbContext.Users.Any()) return;

        var users = new User[]
        {

                new User { Id = 1, Name = "Admin", Username = "admin", Password = "admin", Role = "Admin", Phone_Number = "123-456-7890", Address = "123 Main St" },
                new User { Id = 2, Name = "User", Username = "user", Password = "user", Role = "User", Phone_Number = "987-654-3210", Address = "456 Elm St" }
            //add other users
        };

        foreach (var user in users)
            dbContext.Users.Add(user);
        dbContext.SaveChanges();
        /////////////////////////////////////////
        if (dbContext.Tastes.Any()) return;

        var tastes = new Taste[]
        {

                    new Taste { Id = 1, taste = "Tomato", Price = 12 },
                   new Taste { Id = 2, taste = "Mango", Price = 14 },
                   new Taste { Id = 3, taste = "Chilly", Price = 16 },
                   new Taste { Id = 4, taste = "Popcorn", Price = 18 }
            //add other users
        };

        foreach (var taste in tastes)
            dbContext.Tastes.Add(taste);
        dbContext.SaveChanges();
        /////////////////////////////////////////
        if (dbContext.Flavors.Any()) return;

        var flavors = new Flavor[]
        {

                  new Flavor { Id = 1, flavor = "Spicy", Price = 2 },
                   new Flavor { Id = 2, flavor = "Sweet", Price = 4 },
                   new Flavor { Id = 3, flavor = "Sour", Price = 6 },
                   new Flavor { Id = 4, flavor = "Salty", Price = 8 }
            //add other users
        };

        foreach (var flavor in flavors)
            dbContext.Flavors.Add(flavor);
        dbContext.SaveChanges();
    }
}