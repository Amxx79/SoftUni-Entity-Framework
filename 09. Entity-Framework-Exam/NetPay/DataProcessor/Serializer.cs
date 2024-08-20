using Boardgames.Helpers;
using NetPay.Data;
using NetPay.Data.Models.Enums;
using NetPay.DataProcessor.ExportDtos;
using System.Runtime.Serialization;

namespace NetPay.DataProcessor
{
    public class Serializer
    {
        public static string ExportHouseholdsWhichHaveExpensesToPay(NetPayContext context)
        {
            var paidStatus = PaymentStatus.Paid;

            var houseHoldsAnonymous = context.Households
            .Where(h => h.Expenses.Any(e => e.PaymentStatus != paidStatus))
            .Select(h => new
            {
                ContactPerson = h.ContactPerson,
                Email = h.Email,
                PhoneNumber = h.PhoneNumber,
                Expenses = h.Expenses
                .ToArray()
                .OrderBy(e => e.DueDate)
                .ThenBy(e => e.Amount)
                .Select(e => new
                {
                    ExpenseName = e.ExpenseName,
                    Amount = e.Amount,
                    PaymentDate = e.DueDate,
                    ServiceName = e.Service.ServiceName,
                })
                .ToList()
            })
            .OrderBy(h => h.ContactPerson)
            .ToArray();



            var houseHolds = houseHoldsAnonymous.Select(h => new HouseHoldDto()
            {
                ContactPerson = h.ContactPerson,
                Email = h.Email,
                PhoneNumber = h.PhoneNumber,
                Expenses = h.Expenses
                .ToArray()
                .OrderBy(e => e.PaymentDate)
                .ThenBy(e => e.Amount)
                .Select(e => new ExpensesDto()
                {
                    ExpenseName = e.ExpenseName,
                    Amount = e.Amount,
                    PaymentDate = e.PaymentDate.ToString("yyyy-MM-dd"),
                    ServiceName = e.ServiceName, 
                })
                .ToList()
                .OrderBy(h => h.PaymentDate)
                .ThenBy(e => e.Amount)
                .ToList()
            })
            .OrderBy(h => h.ContactPerson)
            .ToArray();

            return XmlSerializationHelper.Serialize(houseHolds, "Households", false);
        }

        public static string ExportAllServicesWithSuppliers(NetPayContext context)
        {
            throw new NotImplementedException();
        }
    }
}
