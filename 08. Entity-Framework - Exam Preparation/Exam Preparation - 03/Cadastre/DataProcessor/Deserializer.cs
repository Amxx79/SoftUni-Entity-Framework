namespace Cadastre.DataProcessor
{
    using Boardgames.Helper;
    using Cadastre.Data;
    using Cadastre.Data.Enumerations;
    using Cadastre.Data.Models;
    using Cadastre.DataProcessor.ImportDtos;
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Text;

    public class Deserializer
    {
        private const string ErrorMessage =
            "Invalid Data!";
        private const string SuccessfullyImportedDistrict =
            "Successfully imported district - {0} with {1} properties.";
        private const string SuccessfullyImportedCitizen =
            "Succefully imported citizen - {0} {1} with {2} properties.";

        public static string ImportDistricts(CadastreContext dbContext, string xmlDocument)
        {
            var sb = new StringBuilder();

            var deserializedObjects = XmlSerializationHelper.Deserialize<ImportDistrictDto[]>(xmlDocument, "Districts");

            ICollection<District> districtsToImport = new List<District>();

            foreach (var dto in deserializedObjects)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage); 
                    continue;
                }

                if (dbContext.Districts.Any(d => d.Name.Contains(dto.Name)))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                District newDistrict = new District()
                {
                    Name = dto.Name,
                    PostalCode = dto.PostalCode,
                    Region = dto.Region,
                };

                foreach (var prop in dto.Properties)
                {
                    if (!IsValid(prop))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (newDistrict.Properties.Any(p => p.PropertyIdentifier.Contains(prop.PropertyIdentifier)) ||
                        dbContext.Properties.Any(p => p.PropertyIdentifier.Contains(prop.PropertyIdentifier)))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (newDistrict.Properties.Any(p => p.Address.Contains(prop.Address)) ||
                        dbContext.Properties.Any(p => p.Address.Contains(prop.Address)))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    DateTime acquisitionDate = DateTime.ParseExact(prop.DateOfAcquisition, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None);

                    Property newProperty = new Property()
                    {
                        PropertyIdentifier = prop.PropertyIdentifier,
                        Area = prop.Area,
                        Details = prop.Details,
                        Address = prop.Address,
                        DateOfAcquisition = acquisitionDate,
                    };
                    newDistrict.Properties.Add(newProperty);
                }


                districtsToImport.Add(newDistrict);
                sb.AppendLine(String.Format
                    (SuccessfullyImportedDistrict, newDistrict.Name, newDistrict.Properties.Count()));
            }

            dbContext.Districts.AddRange(districtsToImport);
            dbContext.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportCitizens(CadastreContext dbContext, string jsonDocument)
        {
            var sb = new StringBuilder();

            ICollection<Citizen> citizensToImport = new List<Citizen>();

            var deserializedObjects = JsonConvert.DeserializeObject<ImportCitizenDto[]>(jsonDocument);

            foreach (var dto in deserializedObjects)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Citizen newCitizen = new Citizen()
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    BirthDate = DateTime.ParseExact(dto.BirthDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None),
                    MaritalStatus = (MaritalStatus)Enum.Parse(typeof(MaritalStatus), dto.MaritalStatus),
                };

                foreach (var prop in dto.Properties)
                {
                    var newPropertyCitizen = new PropertyCitizen()
                    {
                        PropertyId = prop,
                        Citizen = newCitizen,
                    };

                    newCitizen.PropertiesCitizens.Add(newPropertyCitizen);
                }
                sb.AppendLine(String.Format
                    (SuccessfullyImportedCitizen, newCitizen.FirstName, newCitizen.LastName, newCitizen.PropertiesCitizens.Count()));

                citizensToImport.Add(newCitizen);
            }

            dbContext.AddRange(citizensToImport);
            dbContext.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}
