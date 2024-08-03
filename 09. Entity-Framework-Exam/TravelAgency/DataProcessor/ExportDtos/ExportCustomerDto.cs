using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelAgency.DataProcessor.ExportDtos
{
    public class ExportCustomerDto
    {
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        [JsonProperty("Bookings")]
        public ExportBookingDto[] Bookings { get; set; }
    }
}
