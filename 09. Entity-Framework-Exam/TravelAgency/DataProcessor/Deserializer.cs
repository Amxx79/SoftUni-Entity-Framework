using Boardgames.Helper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
using System.Text;
using TravelAgency.Data;
using TravelAgency.Data.Models;
using TravelAgency.DataProcessor.ImportDtos;

namespace TravelAgency.DataProcessor
{
    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data format!";
        private const string DuplicationDataMessage = "Error! Data duplicated.";
        private const string SuccessfullyImportedCustomer = "Successfully imported customer - {0}";
        private const string SuccessfullyImportedBooking = "Successfully imported booking. TourPackage: {0}, Date: {1}";

        public static string ImportCustomers(TravelAgencyContext context, string xmlString)
        {
            var sb = new StringBuilder();

            var deserializedObjects = (ImportCustomerDto[])XmlSerializationHelper.Deserialize<ImportCustomerDto[]>(xmlString, "Customers");

            ICollection<Customer> customersToImport = new List<Customer>();
            var customerInDatabase = context.Customers.ToArray();

            foreach(var dto in deserializedObjects)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (customersToImport.Any(c => c.FullName.Contains(dto.FullName)))
                {
                    sb.AppendLine(DuplicationDataMessage);
                    continue;
                }

                if (customersToImport.Any(c => c.FullName == (dto.FullName)) ||
                    customersToImport.Any(c => c.Email == (dto.Email)) ||
                    customersToImport.Any(c => c.PhoneNumber == (dto.PhoneNumber)))
                {
                    sb.AppendLine(DuplicationDataMessage);
                    continue;
                }

                Customer newCustomer = new Customer()
                {
                    FullName = dto.FullName,
                    Email = dto.Email,
                    PhoneNumber = dto.PhoneNumber,
                };

                sb.AppendLine(String.Format(SuccessfullyImportedCustomer, newCustomer.FullName));
                customersToImport.Add(newCustomer);
            }

            context.Customers.AddRange(customersToImport);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportBookings(TravelAgencyContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var deserializedObjects = JsonConvert.DeserializeObject<ImportBookingDto[]>(jsonString);

            var allTourPackages = context.TourPackages;

            ICollection<Booking> bookingsToImport = new HashSet<Booking>();

            foreach (var dto in deserializedObjects)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                bool isValidDate = DateTime.TryParseExact(dto.BookingDate,"yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt);

                Customer currentCustomer = context.Customers.Where(c => c.FullName == dto.CustomerName).FirstOrDefault();
                TourPackage currentTourPackage = context.TourPackages.Where(t => t.PackageName == dto.TourPackageName).FirstOrDefault();

                if (currentCustomer == null || currentTourPackage == null)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (isValidDate == false)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Booking newBooking = new Booking()
                {
                    BookingDate = dt,
                    Customer = currentCustomer,
                    TourPackage = currentTourPackage,
                };

                sb.AppendLine(String.Format(SuccessfullyImportedBooking, dto.TourPackageName, dto.BookingDate));
                bookingsToImport.Add(newBooking);
            }

            context.Bookings.AddRange(bookingsToImport);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static bool IsValid(object dto)
        {
            var validateContext = new ValidationContext(dto);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(dto, validateContext, validationResults, true);

            foreach (var validationResult in validationResults)
            {
                string currValidationMessage = validationResult.ErrorMessage;
            }

            return isValid;
        }
    }
}
