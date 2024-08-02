using Cadastre.Data.Enumerations;
using Cadastre.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Cadastre.DataProcessor.ImportDtos
{
    [XmlType("District")]
    public class ImportDistrictDto
    {
        [Required]
        [MinLength(2)]
        [MaxLength(80)]
        public string Name { get; set; }

        [Required]
        [MinLength(8)]
        [MaxLength(8)]
        [RegularExpression(@"[A-Z]{2}-\d{5}")]
        public string PostalCode { get; set; }

        [XmlAttribute("Region")]
        public Region Region { get; set; }

        public ImportPropertyDto[] Properties { get; set; }
    }
}
