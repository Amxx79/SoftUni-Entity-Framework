using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductShop.DTOs.Export
{
    public class CategoryDTO
    {
        public string Category { get; set; }
        public int ProductsCount { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
