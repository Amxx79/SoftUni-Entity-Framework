using CarDealer.Data;
using CarDealer.DTOs.Export;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
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
            //string suppliersXml = File.ReadAllText("../../../Datasets/cars.xml");
            //Console.WriteLine(ImportCars(context, suppliersXml));

            //12
            //string suppliersXml = File.ReadAllText("../../../Datasets/customers.xml");
            //Console.WriteLine(ImportCustomers(context, suppliersXml));

            //13
            //string suppliersXml = File.ReadAllText("../../../Datasets/sales.xml");
            //Console.WriteLine(ImportSales(context, suppliersXml));

            //14
            //Console.WriteLine(GetCarsWithDistance(context));

            //15
            //Console.WriteLine(GetCarsFromMakeBmw(context));

            //16
            //Console.WriteLine(GetLocalSuppliers(context));

            //17
            //Console.WriteLine(GetCarsWithTheirListOfParts(context));

            //18
            //Console.WriteLine(GetTotalSalesByCustomer(context));

            //19
            Console.WriteLine(GetSalesWithAppliedDiscount(context));

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
       new XmlRootAttribute("Customers"));

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

        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(SalesImportDto[]),
       new XmlRootAttribute("Sales"));

            SalesImportDto[] salesDtos;
            using (var reader = new StringReader(inputXml))
            {
                salesDtos = (SalesImportDto[])xmlSerializer.Deserialize(reader);
            }

            int[] validIds = context.Cars
                .Select(c => c.Id).
                ToArray();


            Sale[] sales = salesDtos
                .Where(s => validIds.Contains(s.CarId))
                .Select(s => new Sale
                {
                    CarId = s.CarId,
                    CustomerId = s.CustomerId,
                    Discount = s.Discount,
                })
                .ToArray();

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Length}";
        }

        public static string GetCarsWithDistance(CarDealerContext context)
        {
            var cars = context.Cars
                .Select(c => new CarExportDto
                {
                    Make = c.Make,
                    Model = c.Model,
                    TraveledDistance = c.TraveledDistance,
                })
                .OrderBy(c => c.Make)
                .ThenBy(c => c.Model)
                .Take(10)
                .ToArray();

            return SerializeToXml(cars, "cars");
        }

        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            var bmws = context.Cars
                .Where(c => c.Make == "BMW")
                .Select(b => new BmwExportDto()
                {
                    Id = b.Id,
                    Model = b.Model,
                    TraveledDistance = b.TraveledDistance,
                })
                .OrderBy(b => b.Model)
                .ThenByDescending(b => b.TraveledDistance)
                .ToArray();

            return SerializeToXml(bmws, "cars", true);
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(s => s.IsImporter == false)
                .Select(s => new LocalSupplierDto()
                {
                    Id = s.Id,
                    Name = s.Name,
                    PartsCount = s.Parts.Count()
                })
                .ToArray();

            return SerializeToXml(suppliers, "suppliers");
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var carsWithParts = context.Cars
                .OrderByDescending(c => c.TraveledDistance)
                .ThenBy(c => c.Model)
                .Take(5)
                .Select(c => new CarWithParts()
                {
                    Make = c.Make,
                    Model = c.Model,
                    TraveledDistance = c.TraveledDistance,
                    Parts = c.PartsCars
                        .OrderByDescending(p => p.Part.Price)
                        .Select(p => new PartsForCarsDto()
                        {
                            Name = p.Part.Name,
                            Price = p.Part.Price,
                        })
                        .ToArray()
                })
                .ToArray();

            return SerializeToXml(carsWithParts, "cars");
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var temp = context.Customers
                .Where(c => c.Sales.Any())
                .Select(c => new
                {
                    FullName = c.Name,
                    BoughtCars = c.Sales.Count(),
                    SalesInfo = c.Sales
                    .Select(s => new
                    {
                        Prices = c.IsYoungDriver
                        ? s.Car.PartsCars.Sum(pc => Math.Round((double)pc.Part.Price * 0.95, 2))
                        : s.Car.PartsCars.Sum(pc => (double)pc.Part.Price)
                    }).ToArray()
                })
                .ToArray();

            var customerInfo = temp
                .OrderByDescending(x => x.SalesInfo
                .Sum(y => y.Prices))
                .Select(x => new CustomerDto()
                {
                    FullName = x.FullName,
                    CarsBought = x.BoughtCars,
                    MoneySpent = x.SalesInfo.Sum(b => (decimal)b.Prices)
                })
                .ToArray();

            return SerializeToXml(customerInfo, "customers");
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                .Select(s => new SaleDto()
                {
                    Car = new CarDto()
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TraveledDistance = s.Car.TraveledDistance,
                    },
                    Discount = (int)s.Discount,
                    CustomerName = s.Customer.Name,
                    Price = s.Car.PartsCars.Sum(pc => pc.Part.Price),
                    PriceWithDiscount = Math.Round((double)(s.Car.PartsCars.Sum(pc => pc.Part.Price)
                    * (1 - (s.Discount / 100))), 4),
                }).ToArray();

            return SerializeToXml(sales, "sales");
        }

        //Serialize (Skip it)
        private static string SerializeToXml<T>(T dto, string xmlRootAttribute, bool omitDeclaration = false)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T), new XmlRootAttribute(xmlRootAttribute));
            StringBuilder stringBuilder = new StringBuilder();

            XmlWriterSettings settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = omitDeclaration,
                Encoding = new UTF8Encoding(false),
                Indent = true
            };

            using (StringWriter stringWriter = new StringWriter(stringBuilder, CultureInfo.InvariantCulture))
            using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, settings))
            {
                XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
                xmlSerializerNamespaces.Add(string.Empty, string.Empty);

                try
                {
                    xmlSerializer.Serialize(xmlWriter, dto, xmlSerializerNamespaces);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            return stringBuilder.ToString();
        }
    }
}
