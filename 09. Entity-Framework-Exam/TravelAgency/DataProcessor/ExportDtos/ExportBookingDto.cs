using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelAgency.DataProcessor.ExportDtos
{
    public class ExportBookingDto
    {
        [JsonProperty("TourPackageName")]
        public string TourPackageName { get; set; }
        [JsonProperty("Date")]
        public string Date { get; set; }
    }
}
