using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelAgency.DataProcessor.ImportDtos
{
    [JsonObject("Bookings")]
    public class ImportBookingDto
    {
        [JsonProperty("BookingDate")]
        [Required]
        public string BookingDate { get; set; }

        [JsonProperty("CustomerName")]
        public string CustomerName { get; set; }

        [JsonProperty("TourPackageName")]
        public string TourPackageName { get; set; }
    }
}
