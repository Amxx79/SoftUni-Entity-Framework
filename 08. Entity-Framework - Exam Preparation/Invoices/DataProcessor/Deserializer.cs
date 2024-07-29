namespace Invoices.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using System.Xml.Serialization;
    using Invoices.Data;
    using Invoices.Data.Models;
    using Invoices.DataProcessor.ImportDto;
    using Invoices.Utilities;

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
            throw new NotImplementedException();
        }

        public static string ImportProducts(InvoicesContext context, string jsonString)
        {


            throw new NotImplementedException();
        }

        public static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    } 
}
