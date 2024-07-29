using System.ComponentModel.DataAnnotations;
using static Invoices.Data.DataConstraints;

namespace Invoices.Data.Models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; }
        [Required]
        [MaxLength(NumberVatMaxLength)]
        public string NumberVat { get; set; }
        public virtual ICollection<ProductClient> ProductsClients { get; set; } = new List<ProductClient>();
        public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();
        public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}
