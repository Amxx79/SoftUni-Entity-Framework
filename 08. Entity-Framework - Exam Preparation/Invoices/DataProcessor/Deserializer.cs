namespace Invoices.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Text;
    using System.Xml.Serialization;
    using Invoices.Data;
    using Invoices.Data.Models;
    using Invoices.Data.Models.Enums;
    using Invoices.DataProcessor.ImportDto;
    using Invoices.Utilities;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedClients
            = "Successfully imported client {0}.";

        private const string SuccessfullyImportedInvoices
            = "Successfully imported invoice with number {0}.";

        private const string SuccessfullyImportedProducts
            = "Successfully imported product - {0} with {1} clients.";


        public static string ImportClients(InvoicesContext context, string xmlString)
        {
            XmlHelper helper = new XmlHelper();
            string xmlRoot = "Clients";

            ImportClientDto[] clientDtos = helper.Deserialize<ImportClientDto[]>(xmlString, xmlRoot);

            var sb = new StringBuilder();

            ICollection<Client> clientsToImport = new List<Client>();

            foreach (var dto in clientDtos)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                ICollection<Address> addressesFromDto = new List<Address>();

                foreach (var addr in dto.Addresses)
                {
                    if (!IsValid(addr))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    Address newAddress = new Address()
                    {
                        StreetName = addr.StreetName,
                        StreetNumber = addr.StreetNumber,
                        PostCode = addr.PostCode,
                        City = addr.City,
                        Country = addr.Country,
                    };
                    addressesFromDto.Add(newAddress);
                }

                Client newClient = new Client()
                {
                    Name = dto.Name,
                    NumberVat = dto.NumberVat,
                    Addresses = addressesFromDto,
                };

                clientsToImport.Add(newClient);
                sb.AppendLine(String.Format(SuccessfullyImportedClients, newClient.Name));
            }
            context.Clients.AddRangeAsync(clientsToImport);
            context.SaveChanges();
            return sb.ToString().Trim();
        }


        public static string ImportInvoices(InvoicesContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            ImportInvoiceDto[] invoicesDtos = JsonConvert.DeserializeObject<ImportInvoiceDto[]>(jsonString);

            ICollection<Invoice> invoicesToImport = new List<Invoice>();

            foreach (var invoice in invoicesDtos)
            {
                if (!IsValid(invoice))
                {
                    sb.AppendLine(ErrorMessage); 
                    continue;
                }

                bool isDateCorrect = DateTime.TryParse(invoice.IssueDate, CultureInfo.InvariantCulture, DateTimeStyles.None,
                    out DateTime issueDate);

                bool isDueDateCorrect = DateTime.TryParse(invoice.DueDate, CultureInfo.InvariantCulture, DateTimeStyles.None,
                    out DateTime dueDate);

                if (isDateCorrect == false || isDueDateCorrect == false || DateTime.Compare(dueDate, issueDate) < 0)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (!context.Clients.Any(c => c.Id == invoice.ClientId))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Invoice newInvoice = new Invoice()
                {
                    Number = invoice.Number,
                    IssueDate = issueDate,
                    DueDate = dueDate,
                    Amount = invoice.Amount,
                    CurrencyType = (CurrencyType)invoice.CurrencyType,
                    ClientId = invoice.ClientId
                };

                invoicesToImport.Add(newInvoice);
                sb.AppendLine(String.Format(SuccessfullyImportedInvoices, invoice.Number));
            }

            context.Invoices.AddRange(invoicesToImport);
            context.SaveChanges();
            return sb.ToString().Trim();
        }

        public static string ImportProducts(InvoicesContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();
            ICollection<Product> productToImport = new List<Product>();

            ImportProductDto[] deserializedProducts = JsonConvert.DeserializeObject<ImportProductDto[]>(jsonString);

            foreach (var dto in deserializedProducts)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage); 
                    continue;
                }

                Product newProduct = new Product()
                {
                    Name = dto.Name,
                    Price = dto.Price,
                    CategoryType = (CategoryType)dto.CategoryType
                };

                ICollection<ProductClient> productClient = new List<ProductClient>();
                foreach (var client in dto.Clients.Distinct())
                {
                    if (!context.Clients.Any(cl => cl.Id == client))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    ProductClient newProductClient = new ProductClient()
                    {
                        ClientId = client,
                        Product = newProduct,
                    };

                    productClient.Add(newProductClient);
                }

                newProduct.ProductsClients = productClient;

                productToImport.Add(newProduct);
                sb.AppendLine(String.Format(SuccessfullyImportedProducts, dto.Name, newProduct.ProductsClients.Count()));
            }

            context.Products.AddRange(productToImport);
            context.SaveChanges();
            return sb.ToString().Trim();
        }

        public static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    } 
}
