
using System.Runtime.Serialization;
using System.Collections.ObjectModel;

namespace Amortization.RequestResponse
{
    [DataContract]
    public class CalculateAmortizationResponse
    {
        [DataMember]
        public ObservableCollection<PaymentDetail> PaymentDetails { set; get; }

        public CalculateAmortizationResponse()
        {
            PaymentDetails = new ObservableCollection<PaymentDetail>();
        }
    }
}
