using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models
{
    public class Payment
    {
        private int PaymentId { get; set; }
        private decimal Amount { get; set; }
        private DateTime PaymentDate { get; set; }
        private string MethodOfPayment { get; set; } = string.Empty;
        private int ReservationId { get; set; }

    }
}
