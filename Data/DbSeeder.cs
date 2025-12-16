using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using CardCore.Constants;

namespace CardCore.Data
{
    public class DbSeeder
    {

        public static async Task SeedDefaultData(IServiceProvider service)
        {
            try
            {
                var context = service.GetService<ApplicationDbContext>();

                // this block will check if there are any pending migrations and apply them
                if ((await context.Database.GetPendingMigrationsAsync()).Count() > 0)
                {
                    await context.Database.MigrateAsync();
                }

                //====================================add admin and users//==========================================
                var userMgr = service.GetService<UserManager<IdentityUser>>();
                var roleMgr = service.GetService<RoleManager<IdentityRole>>();
                // create admin role if not exists
                var adminRoleExists = await roleMgr.RoleExistsAsync(Roles.Admin.ToString());
                if (!adminRoleExists)
                {
                    await roleMgr.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
                }
                // create user role if not exists
                var userRoleExists = await roleMgr.RoleExistsAsync(Roles.User.ToString());

                if (!userRoleExists)
                {
                    await roleMgr.CreateAsync(new IdentityRole(Roles.User.ToString()));
                }
                // adding some roles to db
                await roleMgr.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
                await roleMgr.CreateAsync(new IdentityRole(Roles.User.ToString()));

                // create admin user
                var admin = new IdentityUser
                {
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    EmailConfirmed = true
                };

                var userInDb = await userMgr.FindByEmailAsync(admin.Email);
                if (userInDb is null)
                {
                    await userMgr.CreateAsync(admin, "Admin@123");
                    await userMgr.AddToRoleAsync(admin, Roles.Admin.ToString());
                }
                //=====================================================================================================

                if (!context.Genres.Any())
                {
                    await SeedGenreAsync(context);
                }

                if (!context.Products.Any())
                {
                    await SeedProductsAsync(context);
                    // update stock table
                    await context.Database.ExecuteSqlRawAsync(@"
                     INSERT INTO Stock(ProductId,Quantity) 
                     SELECT 
                     b.Id,
                     10 
                     FROM Product b
                     WHERE NOT EXISTS (
                     SELECT * FROM [Stock]
                     );
                    ");
                }


            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


        }

        #region private methods
        private static async Task SeedGenreAsync(ApplicationDbContext context)
        {
            var genres = new[]
            {
                new Genre { GenreName = "Furniture" },
                new Genre { GenreName = "Shirt" },
                new Genre { GenreName = "Shoe" },
                new Genre { GenreName = "SmartPhone" },
                new Genre { GenreName = "Tv" },
                new Genre { GenreName = "Fastfood" },
            };

            await context.Genres.AddRangeAsync(genres);
            await context.SaveChangesAsync();
        }

        private static async Task SeedProductsAsync(ApplicationDbContext context)
        {
            var products = new List<Product>
        {
        // Furniture (GenreId = 1)
        new Product { ProductName = "Pride and Prejudice", CompanyName = "Jane Austen Ltd.", Price = 12005, GenreId = 1, Image = "Chirpexels.jpg", ReleaseDate = DateTime.Now.AddDays(-5), OnSale = true },
        new Product { ProductName = "The Notebook", CompanyName = "Nicholas House", Price = 11005, GenreId = 1, Image = "armchair.jpg", ReleaseDate = DateTime.Now.AddDays(-10),OnSale = false},
        new Product { ProductName = "Outlander", CompanyName = "Diana Publishers", Price = 14003, GenreId = 1, Image = "FurnitureSofa.jpg", ReleaseDate = DateTime.Now.AddDays(-15),OnSale = true},
        new Product { ProductName = "Me Before You", CompanyName = "Jojo Bos", Price = 105000, GenreId = 1, Image = "Luxury-Bedroom-Furniture-Set.jpg", ReleaseDate = DateTime.Now.AddDays(-20),OnSale = true},
        new Product { ProductName = "The Fault in Our Stars", CompanyName = "John Green", Price = 90004, GenreId = 1, Image = "product-15-furniture.jpg", ReleaseDate = DateTime.Now.AddDays(-25),OnSale = false},
        new Product { ProductName = "Great Gatsby", CompanyName = "Orwell Co.", Price = 15001, GenreId = 1, Image = "Casa-Padrino-Luxus.jpeg", ReleaseDate = DateTime.Now.AddDays(-30),OnSale = true},
        // Shirt (GenreId = 2)
        new Product { ProductName = "To Kill a Mockingbird", CompanyName = "Harper Lee", Price = 1320, GenreId = 2, Image = "Shirt1.jpg", ReleaseDate = DateTime.Now.AddDays(-35),OnSale = true },
        new Product { ProductName = "The Great Gatsby", CompanyName = "Fitzgerald Press", Price = 1400, GenreId = 2, Image = "Shirt2.jpg", ReleaseDate = DateTime.Now.AddDays(-40),OnSale = true },
        new Product { ProductName = "Little Women", CompanyName = "Louisa May Co.", Price = 500, GenreId = 2, Image = "Shirt3.jpg", ReleaseDate = DateTime.Now.AddDays(-45),OnSale = true },
        new Product { ProductName = "Jane Eyre", CompanyName = "Bronte Bo", Price = 6850, GenreId = 2, Image = "Shirt4.jpg", ReleaseDate = DateTime.Now.AddDays(-50),OnSale = true },
        new Product { ProductName = "Jane Eyre", CompanyName = "Bronte ks", Price = 2300, GenreId = 2, Image = "Shirt5.jpg", ReleaseDate = DateTime.Now.AddDays(-50),OnSale = true },
        new Product { ProductName = "Jane Eyre", CompanyName = "Bronte ooks", Price = 2000, GenreId = 2, Image = "Shirt6.jpg", ReleaseDate = DateTime.Now.AddDays(-50),OnSale = true },

        // Shoe (GenreId = 3)
        new Product { ProductName = "Casual Cotton ", CompanyName = "H&M", Price = 202, GenreId = 3, Image = "Shoe1.jpg",  ReleaseDate = DateTime.Now.AddDays(-5),OnSale = true},
        new Product { ProductName = "Formal White ", CompanyName = "Zara", Price = 255, GenreId = 3, Image = "Shoe2.jpg", ReleaseDate = DateTime.Now.AddDays(-10),OnSale = false },
        new Product { ProductName = "Denim Shirt", CompanyName = "Levi's", Price = 300, GenreId = 3, Image = "Shoe3.jpg", ReleaseDate = DateTime.Now.AddDays(-15) , OnSale = true},
        new Product { ProductName = "Denim Shirt", CompanyName = "Li's", Price = 309, GenreId = 3, Image = "Shoe4.jpg", ReleaseDate = DateTime.Now.AddDays(-15) , OnSale = true},
        new Product { ProductName = "Shirt", CompanyName = "vi's", Price = 306, GenreId = 3, Image = "Shoe5.jpg", ReleaseDate = DateTime.Now.AddDays(-15) , OnSale = true},
        new Product { ProductName = "Denim", CompanyName = "evi's", Price = 304, GenreId = 3, Image = "Shoe6.jpg", ReleaseDate = DateTime.Now.AddDays(-15) , OnSale = true},

        // SmartPhone (GenreId = 4)
        new Product { ProductName = "Pride and Prejudice", CompanyName = "Jane Austen Ltd.", Price = 12005, GenreId = 4, Image = "Samsung.jpg", ReleaseDate = DateTime.Now.AddDays(-5), OnSale = true },
        new Product { ProductName = "The Notebook", CompanyName = "Nicholas House", Price = 11005, GenreId = 4, Image = "IPhone.png", ReleaseDate = DateTime.Now.AddDays(-10),OnSale = false},
        new Product { ProductName = "Outlander", CompanyName = "Diana Publishers", Price = 14003, GenreId = 4, Image = "Nokia.jpg", ReleaseDate = DateTime.Now.AddDays(-15),OnSale = true},
        new Product { ProductName = "Me Before You", CompanyName = "Jojo Bos", Price = 105000, GenreId = 4, Image = "walton.jpg", ReleaseDate = DateTime.Now.AddDays(-20),OnSale = true},
        new Product { ProductName = "The Fault in Our Stars", CompanyName = "John Green", Price = 90004, GenreId = 4, Image = "Foldable.jpg", ReleaseDate = DateTime.Now.AddDays(-25),OnSale = false},
        new Product { ProductName = "Great Gatsby", CompanyName = "Orwell Co.", Price = 15001, GenreId = 4, Image = "no-symbol-colorful-no-text-illustration-K5hH2ehj.jpg", ReleaseDate = DateTime.Now.AddDays(-30) , OnSale = true},
        // TV (GenreId = 5)
        new Product { ProductName = "To Kill a Mockingbird", CompanyName = "Harper Lee", Price = 1320, GenreId = 5, Image = "Tv1.jpg", ReleaseDate = DateTime.Now.AddDays(-35) , OnSale = true},
        new Product { ProductName = "The Great Gatsby", CompanyName = "Fitzgerald Press", Price = 1400, GenreId = 5, Image = "Tv3.jpg", ReleaseDate = DateTime.Now.AddDays(-40) , OnSale = true},
        new Product { ProductName = "Little Women", CompanyName = "Louisa May Co.", Price = 500, GenreId = 5, Image = "TV4.jpg", ReleaseDate = DateTime.Now.AddDays(-45) , OnSale = true},
        new Product { ProductName = "Jane Eyre", CompanyName = "Bronte Bo", Price = 6850, GenreId = 5, Image = "tv5.jpg", ReleaseDate = DateTime.Now.AddDays(-50) , OnSale = true},
        new Product { ProductName = "Jane Eyre", CompanyName = "Bronte ks", Price = 2300, GenreId = 5, Image = "Tv6.png", ReleaseDate = DateTime.Now.AddDays(-50) , OnSale = true},
        new Product { ProductName = "Jane Eyre", CompanyName = "Bronte ooks", Price = 2000, GenreId = 5, Image = "Ximi TV .jpg", ReleaseDate = DateTime.Now.AddDays(-50) , OnSale = true},
        };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();

            var productInformations = new List<ProductInformation>
            {
                new ProductInformation{Description = "dgfagy fgthhh rfgareh",MadeIn = "Bangladesh",ProductId = products[0].Id},
                new ProductInformation{Description = "fghgfjhyj sewqrew jkuyii",MadeIn = "Bangladesh",ProductId = products[1].Id},
                new ProductInformation{Description = "Soft cotton fabric, perfect for summer wear.",MadeIn = "India",ProductId = products[2].Id},
                new ProductInformation{Description = "Durable leather material, stylish and long-lasting.",MadeIn = "Italy",ProductId = products[3].Id},
                new ProductInformation{Description = "Lightweight and comfortable design for daily use.",MadeIn = "China",ProductId = products[4].Id},
                new ProductInformation
                {
                    Description = "Premium cotton T-shirt with soft touch and modern fit.",
                    MadeIn = "Bangladesh",
                    ProductId = products[6].Id
                },
                new ProductInformation
                {
                    Description = "Stylish denim jeans with strong stitches for durability.",
                    MadeIn = "Pakistan",
                    ProductId = products[7].Id
                },
                new ProductInformation
                {
                    Description = "High-quality blazer suitable for formal occasions.",
                    MadeIn = "Turkey",
                    ProductId = products[8].Id
                },
                new ProductInformation
                {
                    Description = "Comfortable sneakers with breathable mesh design.",
                    MadeIn = "Vietnam",
                    ProductId = products[9].Id
                },
                new ProductInformation
                {
                    Description = "Traditional Panjabi made with handwoven cotton.",
                    MadeIn = "Bangladesh",
                    ProductId = products[10].Id
                },
                new ProductInformation
                {
                    Description = "Smartphone with high-speed processor and AMOLED display.",
                    MadeIn = "China",
                    ProductId = products[11].Id
                },
                new ProductInformation
                {
                    Description = "LED TV with 4K UHD resolution and smart features.",
                    MadeIn = "Malaysia",
                    ProductId = products[12].Id
                },
                new ProductInformation
                {
                    Description = "Lightweight laptop with long battery life and SSD storage.",
                    MadeIn = "Taiwan",
                    ProductId = products[25].Id
                },
                new ProductInformation
                {
                    Description = "Modern wooden furniture with polished finish.",
                    MadeIn = "Indonesia",
                    ProductId = products[14].Id
                },
                new ProductInformation
                {
                    Description = "Luxury sofa set with velvet fabric and elegant design.",
                    MadeIn = "Italy",
                    ProductId = products[20].Id
                },

            };

            await context.ProductInformations.AddRangeAsync(productInformations);
            await context.SaveChangesAsync();
        }

        #endregion
    }


}


