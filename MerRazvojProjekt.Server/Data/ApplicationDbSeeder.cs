using MerRazvojProjekt.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace MerRazvojProjekt.Server.Data
{
    public static class ApplicationDbSeeder
    {
        public static async Task SeedCustomersAsync(ApplicationDbContext context)
        {
            if (await context.Customers.AnyAsync())
                return;

            var firstNames = new[]
            {
                "Ivan", "Ana", "Marko", "Petra", "Luka", "Mia", "Filip", "Ema", "Karlo", "Iva",
                "Nikola", "Lucija", "Josip", "Sara", "Matej", "Lea", "David", "Nika", "Tomislav", "Dora"
            };

            var lastNames = new[]
            {
                "Horvat", "Kovacic", "Babic", "Maric", "Novak", "Knezevic", "Jukic", "Kovac",
                "Pavic", "Bozic", "Milic", "Peric", "Grgic", "Vidovic", "Tomic", "Vukovic",
                "Radić", "Lovric", "Sekulic", "Matic"
            };

            var locations = new (string City, string Country)[]
            {
                ("Zagreb", "Croatia"),
                ("Split", "Croatia"),
                ("Rijeka", "Croatia"),
                ("Osijek", "Croatia"),
                ("Zadar", "Croatia"),
                ("Ljubljana", "Slovenia"),
                ("Maribor", "Slovenia"),
                ("Sarajevo", "Bosnia and Herzegovina"),
                ("Mostar", "Bosnia and Herzegovina"),
                ("Belgrade", "Serbia"),
                ("Novi Sad", "Serbia"),
                ("Skopje", "North Macedonia"),
                ("Podgorica", "Montenegro"),
                ("Vienna", "Austria"),
                ("Graz", "Austria")
            };

            const int totalCustomers = 100000;
            const int batchSize = 5000;

            var customers = new List<Customer>(batchSize);

            context.ChangeTracker.AutoDetectChangesEnabled = false;

            try
            {
                for (int i = 1; i <= totalCustomers; i++)
                {
                    var firstName = firstNames[(i - 1) % firstNames.Length];
                    var lastName = lastNames[((i - 1) / firstNames.Length) % lastNames.Length];
                    var location = locations[(i - 1) % locations.Length];

                    customers.Add(new Customer
                    {
                        FirstName = firstName,
                        LastName = lastName,
                        Email = $"{firstName.ToLower()}.{lastName.ToLower()}{i}@example.com",
                        Phone = $"+38591{(100000 + i % 900000):D6}",
                        City = location.City,
                        Country = location.Country,
                        IsActive = i % 5 != 0,
                        CreatedAt = DateTime.UtcNow.AddMinutes(-i),
                        LastModifiedAt = i % 3 == 0 ? DateTime.UtcNow.AddMinutes(-(i / 2)) : null
                    });

                    if (customers.Count == batchSize)
                    {
                        await context.Customers.AddRangeAsync(customers);
                        await context.SaveChangesAsync();
                        customers.Clear();
                        context.ChangeTracker.Clear();
                    }
                }

                if (customers.Count > 0)
                {
                    await context.Customers.AddRangeAsync(customers);
                    await context.SaveChangesAsync();
                    customers.Clear();
                    context.ChangeTracker.Clear();
                }
            }
            finally
            {
                context.ChangeTracker.AutoDetectChangesEnabled = true;
            }
        }


        public static async Task SeedCarsAsync(ApplicationDbContext context)
        {
            if (await context.Cars.AnyAsync())
                return;

            var makes = new[] { "Tesla", "Honda", "Toyota", "Ford", "BMW", "Audi", "Mercedes", "Volkswagen" };

            var carsByMake = new Dictionary<string, string[]>
            {
                ["Tesla"] = ["Model S", "Model 3", "Model X", "Model Y"],
                ["Honda"] = ["Civic", "Accord"],
                ["Toyota"] = ["Corolla", "Camry"],
                ["Ford"] = ["Mustang", "F-150", "Focus"],
                ["BMW"] = ["1 Series", "2 Series", "3 Series", "4 Series", "5 Series"],
                ["Audi"] = ["A3", "A4", "A6", "Q5"],
                ["Mercedes"] = ["C-Class", "E-Class", "S-Class"],
                ["Volkswagen"] = ["Golf", "Passat", "Tiguan"],
            };

            var colors = new[]
            {
                 "Black", "White", "Silver", "Gray",
                 "Red", "Blue",
                 "Midnight Blue", "Pearl White", "Metallic Gray",
                 "Dark Green", "Beige", "Orange"
            };

            var years = Enumerable.Range(2005, 20).ToArray();

            decimal GetBasePrice(string make) => make switch
            {
                "Tesla" => 40000m,
                "BMW" => 35000m,
                "Audi" => 37000m,
                "Mercedes" => 42000m,
                "Volkswagen" => 23000m,
                "Ford" => 25000m,
                "Toyota" => 22000m,
                "Honda" => 20000m,
                _ => 20000m
            };

            var random = new Random();
            const int totalCars = 100000;
            const int batchSize = 5000;

            var cars = new List<Car>(batchSize);

            context.ChangeTracker.AutoDetectChangesEnabled = false;

            try
            {
                for(int i = 1; i <= totalCars; i++)
                {
                    var make = makes[(i - 1) % makes.Length];

                    var modelsForMake = carsByMake[make];

                    var model = modelsForMake[(i - 1) % modelsForMake.Length];

                    var color = colors[(i - 1) % colors.Length];

                    var year = years[(i - 1) % years.Length];

                    var price = GetBasePrice(make) + (decimal)random.Next(0, 20000);

                    cars.Add(new Car
                    {
                        Make = make,
                        Model = model,
                        Year = year,
                        Color = color,
                        Price = price,
                        IsActive = i % 10 != 0
                    });

                    if (cars.Count == batchSize)
                    {
                        await context.Cars.AddRangeAsync(cars);
                        await context.SaveChangesAsync();
                        cars.Clear();
                        context.ChangeTracker.Clear();
                    }
                  
                }

                if (cars.Count > 0)
                {
                    await context.Cars.AddRangeAsync(cars);
                    await context.SaveChangesAsync();
                    cars.Clear();
                    context.ChangeTracker.Clear();
                }

            }
            finally
            {
                context.ChangeTracker.AutoDetectChangesEnabled = true;
            }
        }
    }
}