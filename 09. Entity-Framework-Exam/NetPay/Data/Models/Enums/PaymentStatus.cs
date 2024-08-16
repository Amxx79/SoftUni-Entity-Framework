using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetPay.Data.Models.Enums
{
    public enum PaymentStatus
    {
        Paid = 1, 
        Unpaid = 2,
        Overdue = 3,
        Expired = 4
    }
}
