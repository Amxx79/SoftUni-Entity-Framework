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

            //04
            //var json = File.ReadAllText("../../../Datasets/cars.json");
            //Console.WriteLine(ImportCars(context, json));

            ////05
            //var json = File.ReadAllText("../../../Datasets/customers.json");
            //Console.WriteLine(ImportCustomers(context, json));


            //06
            //var json = File.ReadAllText("../../../Datasets/sales.json");
            //Console.WriteLine(ImportSales(context, json));

            Console.WriteLine(GetCarsFromMakeToyota(context));
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
                    BirthDate= c.BirthDate,
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

    }
}