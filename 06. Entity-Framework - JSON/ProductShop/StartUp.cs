using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProductShop.Data;
using ProductShop.DTOs.Export;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {
            ProductShopContext context = new ProductShopContext();

            //01
            //var userResult = File.ReadAllText("../../../Datasets/users.json");
            //Console.WriteLine(ImportUsers(context, userResult));

            //02
            //var userResult = File.ReadAllText("../../../Datasets/products.json");
            //Console.WriteLine(ImportProducts(context, userResult));

            ////03
            //var userResult = File.ReadAllText("../../../Datasets/categories.json");
            //Console.WriteLine(ImportCategories(context, userResult));

            //04
            //var userResult = File.ReadAllText("../../../Datasets/categories-products.json");
            //Console.WriteLine(ImportCategoryProducts(context, userResult));

            //05
            //Console.WriteLine(GetProductsInRange(context));

            //06
            //Console.WriteLine(GetSoldProducts(context));

            //07
            //Console.WriteLine(GetCategoriesByProductsCount(context));

            //08
            Console.WriteLine(GetUsersWithProducts(context));
        }

        //01
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            var users = JsonConvert.DeserializeObject<List<User>>(inputJson);
            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count}";
        }

        //02
        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            var products = JsonConvert.DeserializeObject<List<Product>>(inputJson);
            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count}";
        }

        //03
        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            var categories = JsonConvert.DeserializeObject<List<Category>>(inputJson);

            categories.RemoveAll(c => c.Name == null);

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count}";
        }

        //04
        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            var categoriesProducts = JsonConvert
                .DeserializeObject<List<CategoryProduct>>(inputJson);

            context.CategoriesProducts.AddRange(categoriesProducts);
            context.SaveChanges();

            return $"Successfully imported {categoriesProducts.Count}";
        }

        //05
        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Select(p => new { p.Name, p.Price, Seller = $"{p.Seller.FirstName} {p.Seller.LastName}" })
                .ToList();

            JsonSerializerSettings jsonSettings = JsonSettings();

            var jsonProducts = JsonConvert.SerializeObject(products, jsonSettings);

            return jsonProducts;
        }

        //06
        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(u => u.ProductsSold.Count > 0 && u.ProductsSold.Any(b => b.BuyerId != null))
                .Select(p => new
                {
                    p.FirstName,
                    p.LastName,
                    SoldProducts = p.ProductsSold.Select(p => new
                    {
                        p.Name,
                        p.Price,
                        BuyerFirstName = p.Buyer.FirstName,
                        BuyerLastName = p.Buyer.LastName
                    })
                })
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .ToList();


            return JsonConvert.SerializeObject(users ,JsonSettings());
        }

        //07
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                .Select(c => new
                {
                    Category = c.Name,
                    ProductsCount = c.CategoriesProducts.Count,
                    AveragePrice = c.CategoriesProducts.Average(p => p.Product.Price).ToString("F2"),
                    TotalRevenue = c.CategoriesProducts.Sum(p => p.Product.Price).ToString("F2"),
                })
                .OrderByDescending(c => c.ProductsCount)
                .ToList();

            return JsonConvert.SerializeObject(categories, JsonSettings());
        }

        //08
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(u => u.ProductsSold.Any(p => p.BuyerId != null && p.Price != null))
                .Select(u => new
                {
                    u.FirstName,
                    u.LastName,
                    u.Age,
                    SoldProducts = u.ProductsSold
                    .Where(p => p.BuyerId != null && p.Price != null)
                    .Select(p => new { p.Name, p.Price })
                })
                .OrderByDescending(u => u.SoldProducts.Count())
                .ToList();

            var output = new
            {
                UsersCount = users.Count,
                Users = users.Select(u => new
                {
                    u.FirstName,
                    u.LastName,
                    u.Age,
                    SoldProducts = new
                    {
                        Count = u.SoldProducts.Count(),
                        Products = u.SoldProducts,
                    }
                })
            };

            return JsonConvert.SerializeObject(output, JsonSettings());
        }


        private static JsonSerializerSettings JsonSettings()
        {
            return new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented,
            };
        }
    }
}