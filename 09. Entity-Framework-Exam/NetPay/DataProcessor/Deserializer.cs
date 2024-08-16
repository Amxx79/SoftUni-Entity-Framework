using Boardgames.Helpers;
using NetPay.Data;
using NetPay.Data.Models;
using NetPay.Data.Models.Enums;
using NetPay.DataProcessor.ImportDtos;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using System.Xml;

namespace NetPay.DataProcessor
{
    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data format!";
        private const string DuplicationDataMessage = "Error! Data duplicated.";
        private const string SuccessfullyImportedHousehold = "Successfully imported household. Contact person: {0}";
        private const string SuccessfullyImportedExpense = "Successfully imported expense. {0}, Amount: {1}";

        public static string ImportHouseholds(NetPayContext context, string xmlString)
        {
            var sb = new StringBuilder();

            var holdersToImport = new List<Household>();

            var importedDtos = (ImportHouseHoldDto[])XmlSerializationHelper.Deserialize<ImportHouseHoldDto[]>(xmlString, "Households");

            foreach(var dto in importedDtos)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage); 
                    continue;
                }

                if (holdersToImport.Any(h => h.Email == dto.Email) ||
                    holdersToImport.Any(h => h.ContactPerson == dto.ContactPerson ||
                    holdersToImport.Any(h => h.PhoneNumber == dto.PhoneNumber)))
                {
                    sb.AppendLine(DuplicationDataMessage);
                    continue;
                }

                var newHouseHolder = new Household()
                {
                    ContactPerson = dto.ContactPerson,
                    PhoneNumber = dto.PhoneNumber,
                    Email = dto.Email,
                };


                holdersToImport.Add(newHouseHolder);
                sb.AppendLine(String.Format(SuccessfullyImportedHousehold, newHouseHolder.ContactPerson));
            }

            context.Households.AddRange(holdersToImport);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportExpenses(NetPayContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var expenseToImport = new List<Expense>();

            var expenseJson = JsonConvert.DeserializeObject<ImportExpenseDto[]>(jsonString);


            foreach (var dto in expenseJson)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage); 
                    continue;
                }

                if (!context.Services.Any(e => e.Id == dto.ServiceId) ||
                    !context.Households.Any(e => e.Id == dto.HouseholdId))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (dto.ExpenseName == null ||
                    dto.Amount == null ||
                    dto.DueDate == null ||
                    dto.PaymentStatus == null ||
                    dto.ServiceId == null ||
                    dto.HouseholdId == null)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                bool isDateDueValid = DateTime.TryParseExact(dto.DueDate, "yyyy-MM-dd",CultureInfo.InvariantCulture, DateTimeStyles.None , out DateTime dateDue);
                var paymentValid = Enum.TryParse(dto.PaymentStatus, out PaymentStatus validPayment);

                if (!isDateDueValid || paymentValid == false) 
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var newExpense = new Expense 
                {
                    ExpenseName = dto.ExpenseName,
                    Amount = dto.Amount,
                    DueDate = dateDue,
                    PaymentStatus = validPayment,
                    HouseholdId = dto.HouseholdId,
                    ServiceId = dto.ServiceId,
                };

                expenseToImport.Add(newExpense);
                sb.AppendLine(String.Format(SuccessfullyImportedExpense, newExpense.ExpenseName, newExpense.Amount));
            }

            context.Expenses.AddRange(expenseToImport);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(dto, validationContext, validationResults, true);

            foreach(var result in validationResults)
            {
                string currvValidationMessage = result.ErrorMessage;
            }

            return isValid;
        }
    }
}
