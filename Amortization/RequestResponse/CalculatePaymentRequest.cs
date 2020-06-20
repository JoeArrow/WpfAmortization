using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Amortization.RequestResponse
{
    [DataContract]
    public class CalculatePaymentRequest
    {
        [DataMember]
        public double AnnualTax { set; get; }
        
        [DataMember]
        public double LoanAmount { set; get; }

        [DataMember]
        public double DownPayment { set; get; }

        private double m_dblInterestRate;

        [DataMember]
        public double InterestRate 
        {
            set 
            {
                // -----------------------------------------------------
                // if value is a not a decimal, convert it to a decimal.
                // 5.75% would become .0575 for example.

                if(value > 1)
                {
                    value /= 100;
                }

                m_dblInterestRate = value; 
            }

            get { return m_dblInterestRate; }
        }

        [DataMember]
        public double NumberOfPayments { set; get; }

        [DataMember]
        public double AnnualInsurancePmt { set; get; }

        // ------------------------------------------------

        public CalculatePaymentRequest()
        {
        }

        // ------------------------------------------------

        public CalculatePaymentRequest(CalculateAmortizationRequest pmtReq)
        {
            AnnualTax = pmtReq.AnnualTax;
            LoanAmount = pmtReq.LoanAmount;
            DownPayment = pmtReq.DownPayment;
            InterestRate = pmtReq.InterestRate;
            NumberOfPayments = pmtReq.NumberOfPayments;
            AnnualInsurancePmt = pmtReq.AnnualInsurancePmt;
        }
    }
}
