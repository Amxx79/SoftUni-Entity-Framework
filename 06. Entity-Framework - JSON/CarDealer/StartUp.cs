using CarDealer.Data;
using CarDealer.DTOs.Export;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using Castle.Core.Resource;
using Castle.DynamicProxy;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.IO;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            var context = new CarDealerContext();


            ////01
            //var json = File.ReadAllText("../../../Datasets/suppliers.json");
            //Console.WriteLine(ImportSuppliers(context, json));


            ////02
            //var json = File.ReadAllText("../../../Datasets/parts.json");
            //Console.WriteLine(ImportParts(context, json));

            ////03
            //var json = File.ReadAllText("../../../Datasets/cars.json");
            //Console.WriteLine(ImportSuppliers(context, json));

            ////04
            //var json = File.ReadAllText("../../../Datasets/cars.json");
            //Console.WriteLine(ImportCars(context, json));

            ////05
            //var json = File.ReadAllText("../../../Datasets/customers.json");
            //Console.WriteLine(ImportCustomers(context, json));


            //06
            //var json = File.ReadAllText("../../../Datasets/sales.json");
            //Console.WriteLine(ImportSales(context, json));

            Console.WriteLine(GetSalesWithAppliedDiscount(context));
        }

        //01
        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var suppliersJson = JsonConvert.DeserializeObject<List<Supplier>>(inputJson);

            context.Suppliers.AddRange(suppliersJson);
            context.SaveChanges();

            return $"Successfully imported {suppliersJson.Count}.";
        }

        //02
        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            var parts = JsonConvert.DeserializeObject<List<Part>>(inputJson);

            var validIds = context.Suppliers
                .Select(x => x.Id)
                .ToList();

            var partWithValidSupplierId = parts
                .Where(p => validIds.Contains(p.SupplierId))
                .ToList();

            context.Parts.AddRange(partWithValidSupplierId);
            context.SaveChanges();

            return $"Successfully imported {partWithValidSupplierId.Count}.";
        }

        //03
        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var carDtos = JsonConvert.DeserializeObject<List<ImportCarDto>>(inputJson);
            var cars = new HashSet<Car>();
            var partsCars = new HashSet<PartCar>();

            foreach (var carDto in carDtos)
            {
                var car = new Car()
                {
                    Make = carDto.Make,
                    Model = carDto.Model,
                    TraveledDistance = carDto.TravelledDistance,
                };

                cars.Add(car);
                foreach (var partId in carDto.PartsId.Distinct())
                {
                    partsCars.Add(new PartCar()
                    {
                        Car = car,
                        PartId = partId,
                    });
                }
            }

            context.Cars.AddRange(cars);
            context.PartsCars.AddRange(partsCars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}.";
        }

        //04
        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var customers = JsonConvert.DeserializeObject<List<Customer>>(inputJson);

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count}.";
        }

        //05
        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            var sales = JsonConvert.DeserializeObject<List<Sale>>(inputJson);

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count}.";
        }

        //III Part - Export the data
        //06
        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var allCustomers = context.Customers
                .OrderBy(c => c.BirthDate)
                .ThenBy(c => !c.IsYoungDriver)
                .Select(c => new CustomerExportDto()
                {
                    Name = c.Name,
                    BirthDate = c.BirthDate,
                    IsYoungDriver = c.IsYoungDriver,
                });

            foreach (var customer in allCustomers)
            {
                customer.BirthDate.ToShortDateString();
            }

            var jsonSettings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented,
                DateFormatString = "dd-MM-yyyy",
            };

            var json = JsonConvert.SerializeObject(allCustomers, jsonSettings);

            return json;
        }

        //07
        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var allCars = context.Cars
                .Where(c => c.Make == "Toyota")
                .OrderBy(c => c)
                .ThenBy(c => c.TraveledDistance)
                .ToList();

            var jsonSettings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented,
                DateFormatString = "dd-MM-yyyy",
            };

            var json = JsonConvert.SerializeObject(allCars, jsonSettings);

            return json;
        }

        //08
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var jsonSettings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented,
                DateFormatString = "dd-MM-yyyy",
            };

            var supp = context.Suppliers.Where(s => !s.IsImporter)
                .Select(s => new
                {
                    Id = s.Id,
                    Name = s.Name,
                    PartsCount = s.Parts.Count(),
                })
                .ToArray();

            return JsonConvert.SerializeObject(supp, jsonSettings);
        }

        //09
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                .Select(c => new
                {
                    car = new
                    {
                        c.Make,
                        c.Model,
                        c.TraveledDistance
                    },
                    parts = c.PartsCars.Select(p => new
                    {
                        Name = p.Part.Name,
                        Price = p.Part.Price.ToString("f2"),
                    })
                    .ToArray(),
                })
                .ToArray();

            var json = JsonConvert.SerializeObject(cars, Formatting.Indented);

            return json;
        }

        //10
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                .Where(c => c.Sales.Count > 0)
                .Select(cs => new
                {
                    fullName = cs.Name,
                    boughtCars = cs.Sales.Count,
                    spentMoney = cs.Sales.Sum(c => c.Car.PartsCars.Sum(c => c.Part.Price)),
                })
                .OrderByDescending(c => c.spentMoney)
                .ThenByDescending(c => c.boughtCars)
                .ToList();

            return JsonConvert.SerializeObject(customers, Formatting.Indented);
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                .Take(10)
                .Select(s => new
                {
                    car = new
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TraveledDistance = s.Car.TraveledDistance,
                    },
                    customerName = s.Customer.Name,
                    discount = s.Discount.ToString("f2"),
                    price = s.Car.PartsCars.Sum(pc => pc.Part.Price).ToString("F2"),
                    priceWithDiscount = (s.Car.PartsCars.Sum(pc => pc.Part.Price) * (1 - s.Discount / 100))
                    .ToString("F2")
                });

            return JsonConvert.SerializeObject(sales, Formatting.Indented);
        }
    }
}
