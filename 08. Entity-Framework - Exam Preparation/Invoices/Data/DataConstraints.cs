using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoices.Data
{
    public static class DataConstraints
    {
        //Product
        public const byte ProductNameMinLength = 9;
        public const byte ProductNameMaxLength = 30;
        public const string ProductPriceMinValue = "5.00";
        public const string ProductPriceMaxValue = "1000.00";

        //Address1
        public const byte StreetNameMinLength = 10;
        public const byte StreetNameMaxLength = 20;
        public const byte CityMinLength = 5;
        public const byte CityMaxLength = 15;
        public const byte CountryMinLength = 5;
        public const byte CountryMaxLength = 15;

        //Invoice
        public const int NumberMinLength = 1_000_000_000;
        public const int NumberMaxLength = 1_500_000_000;

        //Client
        public const byte NameMinLength = 10;
        public const byte NameMaxLength = 25;
        public const byte NumberVatMinLength = 10;
        public const byte NumberVatMaxLength = 15;
    }
}
