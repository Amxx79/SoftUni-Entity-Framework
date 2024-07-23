using CarDealer.Data;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using System.IO;
using System.Xml.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            var context = new CarDealerContext();

            //09
            //string suppliersXml = File.ReadAllText("../../../Datasets/suppliers.xml");
            //Console.WriteLine(ImportSuppliers(context, suppliersXml));

            //10
            //string suppliersXml = File.ReadAllText("../../../Datasets/parts.xml");
            //Console.WriteLine(ImportParts(context, suppliersXml));

            //11
            string suppliersXml = File.ReadAllText("../../../Datasets/cars.xml");
            Console.WriteLine(ImportCars(context, suppliersXml));

        }

        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(SupplierImportDto[]),
                new XmlRootAttribute("Suppliers"));

            SupplierImportDto[] importDtos;

            using (var stringReader = new StringReader(inputXml))
            {
                importDtos = (SupplierImportDto[])xmlSerializer.Deserialize(stringReader);
            };

            Supplier[] suppliers = importDtos
                .Select(dto => new Supplier
                {
                    Name = dto.Name,
                    IsImporter = dto.IsImporter,
                })
                .ToArray();

            context.Suppliers.AddRangeAsync(suppliers);
            context.SaveChanges();


            return $"Successfully imported {importDtos.Length}";
        }

        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(PartImportDto[]),
        new XmlRootAttribute("Parts"));

            PartImportDto[] dtos;
           using (var reader = new StringReader(inputXml))
           {
                dtos = (PartImportDto[])xmlSerializer.Deserialize(reader);
           }

            var supplierId = context.Suppliers
                 .Select(s => s.Id)
                 .ToArray();

            var partsWithValidIds = dtos
                .Where(p => supplierId.Contains(p.SupplierId))
                .ToArray();

            Part[] partsToInsert = partsWithValidIds
                .Select(dto => new Part
                {
                    Name = dto.Name,
                    Price = dto.Price,
                    Quantity = dto.Quantity,
                    SupplierId = dto.SupplierId,
                })
                .ToArray();

            context.AddRangeAsync(partsToInsert);
            context.SaveChanges();

            return $"Successfully imported {partsToInsert.Length}";
        }

        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(CarImportDto[]),
            new XmlRootAttribute("Cars"));

            CarImportDto[] dtos;
            using (var reader = new StringReader(inputXml))
            {
                dtos = (CarImportDto[])xmlSerializer.Deserialize(reader);
            }

            List<Car> cars = new List<Car>();

            foreach (var dto in dtos)
            {
                Car car = new Car()
                {
                    Make = dto.Make,
                    Model = dto.Model,
                    TraveledDistance = dto.TraveledDistance,
                };

                int[] carPartIds = dto.PartIds
                    .Select(p => p.Id)
                    .Distinct()
                    .ToArray();

                var carParts = new List<PartCar>();
                foreach (var id in carPartIds)
                {
                    carParts.Add(new PartCar()
                    {
                        Car = car,
                        PartId = id,
                    });
                    car.PartsCars = carParts;
                }
                cars.Add(car);
            }
            context.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}";
        }

        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(CustomerImportDto[]),
       new XmlRootAttribute("Customer"));

           CustomerImportDto[] customerDtos;

            using (var reader = new StringReader(inputXml))
            {
                customerDtos = (CustomerImportDto[])xmlSerializer.Deserialize(reader);
            }

            Customer[] customers = customerDtos
                .Select(c => new Customer
                {
                    Name = c.Name,
                    BirthDate = c.BirthDate,
                    IsYoungDriver = c.IsYoungDriver,
                })
                .ToArray();

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Length}";
        }
    }
}