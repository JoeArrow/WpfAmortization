using System;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Amortization.RequestResponse
{
    [DataContract]
    public class CalculatePaymentResponse
    {
        [DataMember]
        public decimal PaymentAmount { set; get; }

        public CalculatePaymentResponse()
        {
            PaymentAmount = 0M;
        }
    }
}
