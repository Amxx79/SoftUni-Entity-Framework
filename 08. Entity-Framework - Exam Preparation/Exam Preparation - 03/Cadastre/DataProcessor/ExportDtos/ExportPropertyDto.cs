using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Cadastre.DataProcessor.ExportDtos
{
    [XmlType("Property")]
    public class ExportPropertyDto
    {
        [XmlAttribute("postal-code")]
        public string PostCode { get; set; }

        public string PropertyIdentifier { get; set; }

        public int Area { get; set; }

        public string DateOfAcquisition { get; set; }
    }
}
