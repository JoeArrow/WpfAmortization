
using System.Runtime.Serialization;

namespace Amortization
{
    [DataContract]
    public class PaymentDetail
    {
        [DataMember]
        public double Tax { set; get; }

        [DataMember]
        public int PaymentNo { set; get; }

        [DataMember]
        public double Payment { set; get; }

        [DataMember]
        public double Balance { set; get; }

        [DataMember]
        public double Interest { set; get; }

        [DataMember]
        public double Insurance { set; get; }
        
        [DataMember]
        public double Principle { set; get; }

        // ------------------------------------------------

        public PaymentDetail()
        {
            Tax = 0d;
            Payment = 0d;
            Balance = 0d;
            Interest = 0d;
            PaymentNo = 0;
            Insurance = 0d;
            Principle = 0d;
        }
    }
}