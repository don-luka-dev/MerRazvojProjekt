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
    }
}