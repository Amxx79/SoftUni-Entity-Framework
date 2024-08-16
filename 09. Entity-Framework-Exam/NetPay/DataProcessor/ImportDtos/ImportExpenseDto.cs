using NetPay.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetPay.DataProcessor.ImportDtos
{
    public class ImportExpenseDto
    {
        [MinLength(5)]
        [MaxLength(50)]
        [Required]
        public string ExpenseName { get; set; }

        [Range(0.01, 100_000)]
        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string DueDate { get; set; }

        [Required]
        public string PaymentStatus { get; set; }

        [Required]
        public int HouseholdId { get; set; }

        [Required]
        public int ServiceId { get; set; }
    }
}
