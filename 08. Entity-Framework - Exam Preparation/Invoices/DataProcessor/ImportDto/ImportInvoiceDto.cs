using System.ComponentModel.DataAnnotations;
using static Invoices.Data.DataConstraints;

namespace Invoices.DataProcessor.ImportDto
{
    public class ImportInvoiceDto
    {
        [Required]
        [Range(NumberMinLength, NumberMaxLength)]
        public int Number { get; set; }
        [Required]
        public string IssueDate { get; set; }
        [Required]
        public string DueDate { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Range(MinCurrencyType, MaxCurrencyType)]
        [Required]
        public int CurrencyType { get; set; }
        [Required]
        public int ClientId { get; set; }
    }
}
