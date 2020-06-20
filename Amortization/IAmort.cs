
using System.ServiceModel;

using Amortization.RequestResponse;

namespace Amortization
{
    [ServiceContract]
    public interface IAmort
    {
        [OperationContract]
        CalculatePaymentResponse CalculatePayment(CalculatePaymentRequest req);

        [OperationContract]
        CalculateAmortizationResponse CalculateAmortization(CalculateAmortizationRequest req);
    }
}
