using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetPay.Data.Models
{
    public class Household
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        [Required]
        public string ContactPerson { get; set; }

        [MaxLength(80)]
        public string? Email { get; set; }

        [MaxLength(15)]
        [Required]

        [RegularExpression(@"\+\d{3}\/\d{3}-\d{6}$")]
        public string PhoneNumber { get; set; }

        public virtual ICollection<Expense> Expenses { get; set; }
    }
}
