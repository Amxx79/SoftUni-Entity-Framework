using NetPay.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NetPay.DataProcessor.ImportDtos
{
    [XmlType("Household")]
    public class ImportHouseHoldDto
    {
        [MinLength(15)]
        [MaxLength(15)]
        [Required]
        [XmlAttribute("phone")]
        [RegularExpression(@"\+\d{3}\/\d{3}-\d{6}$")]
        public string PhoneNumber { get; set; }

        [MinLength(5)]
        [MaxLength(50)]
        [Required]
        [XmlElement(nameof(ContactPerson))]
        public string ContactPerson { get; set; }

        [MinLength(6)]
        [MaxLength(80)]
        [XmlElement(nameof(Email))]
        public string Email { get; set; }
    }
}
