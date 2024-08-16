using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetPay.Data.Models
{
    public class Supplier
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(60)]
        public string SupplierName { get; set; }

        public virtual ICollection<SupplierService> SuppliersServices  { get; set; }
    }
}
