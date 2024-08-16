using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetPay.Data.Models
{
    public class Service
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string ServiceName { get; set; }

        public virtual ICollection<Expense> Expenses { get; set; }

        public virtual ICollection<SupplierService> SuppliersServices  { get; set; }
    }
}
