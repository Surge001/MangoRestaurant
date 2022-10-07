using Mango.PaymentProcessor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mango.PaymentProcessor
{
    public class PaymentProcessor : IPaymentProcessor
    {
        public bool ProcessPayment(PaymentDto dto)
        {
            // ToDo: implement payment logic which charges passed-in credit card and returns
            // true if paid successfully, or false if failed:
            return true;
        }
    }
}
