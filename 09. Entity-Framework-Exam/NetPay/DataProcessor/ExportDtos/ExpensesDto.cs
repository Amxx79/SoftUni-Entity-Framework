using System.Xml;
using System.Xml.Serialization;

namespace NetPay.DataProcessor.ExportDtos
{
    [XmlType("Expense")]
    public class ExpensesDto
    {
        public string ExpenseName { get; set; }
        public decimal Amount { get; set; }
        public string PaymentDate { get; set; }
        public string ServiceName { get; set; }
    }
}