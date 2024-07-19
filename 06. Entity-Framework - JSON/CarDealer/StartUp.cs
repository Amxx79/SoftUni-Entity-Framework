using CarDealer.Data;
using CarDealer.Models;
using Newtonsoft.Json;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            var context = new CarDealerContext();


            //01
            //var json = File.ReadAllText("../../../Datasets/suppliers.json");
            //Console.WriteLine(ImportSuppliers(context, json));

            //02
            var json = File.ReadAllText("../../../Datasets/cars.json");
            Console.WriteLine(ImportSuppliers(context, json));
        }

        //01
        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var suppliersJson = JsonConvert.DeserializeObject<List<Supplier>>(inputJson);

            context.Suppliers.AddRange(suppliersJson);
            context.SaveChanges();

            return $"Successfully imported {suppliersJson.Count}.";
        }

        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var cars = JsonConvert.DeserializeObject<List<Car>>(inputJson);

            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}.";
        }


    }
}