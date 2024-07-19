using Castle.Components.DictionaryAdapter;
using System.Text.Json.Serialization;

namespace CarDealer.Models
{
    public class Customer
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public DateTime BirthDate { get; set; }

        public bool IsYoungDriver { get; set; }


        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public ICollection<Sale> Sales { get; set; } = new List<Sale>(); 
    }
}