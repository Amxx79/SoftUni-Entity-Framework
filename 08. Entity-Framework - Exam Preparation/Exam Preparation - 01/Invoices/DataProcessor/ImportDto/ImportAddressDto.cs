using Invoices.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static Invoices.Data.DataConstraints;

namespace Invoices.DataProcessor.ImportDto
{
    [XmlType(nameof(Address))]
    public class ImportAddressDto
    {
        [XmlElement("StreetName")]
        [Required]
        [MinLength(StreetNameMinLength)]
        [MaxLength(StreetNameMaxLength)]
        public string StreetName { get; set; }

        [XmlElement("StreetNumber")]
        [Required]
        public int StreetNumber { get; set; }

        [XmlElement("PostCode")]
        [Required]
        public string PostCode { get; set; }

        [XmlElement("City")]
        [Required]
        [MinLength(CityMinLength)]
        [MaxLength(CityMaxLength)]
        public string City { get; set; }

        [XmlElement("Country")]
        [Required]
        [MinLength(CountryMinLength)]
        [MaxLength(CountryMaxLength)]
        public string Country { get; set; }
    }
}
