using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows;
using System.Windows.Controls;

using Amortization;
using Amortization.RequestResponse;

namespace WpfAmortization
{
    // ----------------------------------------------------
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public partial class AmortWindow : Window
    {
        private DataGridPrinter gridPrinter;
        private PrintDocument printDocument;
        private CalculateAmortizationResponse response;

        // ------------------------------------------------

        public AmortWindow()
        {
            InitializeComponent();
            
            printDocument = new PrintDocument();
            tbTax.Text = Properties.Settings.Default.Tax.ToString();
            tbAPR.Text = Properties.Settings.Default.Apr.ToString();
            tbPeriod.Text = Properties.Settings.Default.Period.ToString();
            tbAltPmt.Text = Properties.Settings.Default.AltPmt.ToString();
            tbAltStart.Text = Properties.Settings.Default.AltStart.ToString();
            tbInsurance.Text = Properties.Settings.Default.Insurance.ToString();
            tbLoanAmount.Text = Properties.Settings.Default.LoanAmount.ToString();
            tbDownPayment.Text = Properties.Settings.Default.DownPayment.ToString();


            printDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(PrintPage);

            SetupGridPrinter();
        }

        // ------------------------------------------------

        void SetupGridPrinter()
        {
            var font = new Font("Roboto", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            gridPrinter = new DataGridPrinter(dgOut, printDocument, font);
        }

        // ------------------------------------------------

        private void OnCalculate(object sender, RoutedEventArgs e)
        {
            double reader;

            if(!string.IsNullOrEmpty(tbLoanAmount.Text) && double.TryParse(tbLoanAmount.Text, out reader) && reader > 0)
            {
                var svc = new AmortSvc();

                var req = new CalculateAmortizationRequest();

                // ----------------------------------------------------------------------
                // Reader got the Loan Amount value in order to get into this code block.

                req.LoanAmount = reader;
                
                if(double.TryParse(tbTax.Text, out reader)) { req.AnnualTax = reader; }
                if(double.TryParse(tbAPR.Text, out reader)) { req.InterestRate = reader; }
                if(double.TryParse(tbPeriod.Text, out reader)) { req.NumberOfPayments = reader; }
                if(double.TryParse(tbDownPayment.Text, out reader)) { req.DownPayment = reader; }
                if(double.TryParse(tbAltPmt.Text, out reader)) { req.AlternatePaymentAmt = reader; }
                if(double.TryParse(tbInsurance.Text, out reader)) { req.AnnualInsurancePmt = reader; }

                int intReader;

                if(int.TryParse(tbAltStart.Text, out intReader)) { req.AlternatePaymentNo = intReader; }

                response = svc.CalculateAmortization(req);

                var totalPayments = 0d;
                var totalInterest = 0d;
                var totalInsurance = 0d;
                var totalTax = 0d;

                var totalPrinciple = 0d;

                foreach(var detail in response.PaymentDetails)
                {
                    totalPayments += detail.Payment;
                    totalInterest += detail.Interest;
                    totalInsurance += detail.Insurance;
                    totalTax += detail.Tax;
                    totalPrinciple += detail.Principle;
                }

                dgOut.ItemsSource = response.PaymentDetails;

                tbTotalTax.Text = totalTax.ToString("c");
                tbTotalPayments.Text = totalPayments.ToString("c");
                tbTotalInterest.Text = totalInterest.ToString("c");
                tbTotalInsurance.Text = totalInsurance.ToString("c");

                tbNumberOfPayments.Text = response.PaymentDetails.Count.ToString();
                btnPrint.IsEnabled = true;
            }
        }

        // ------------------------------------------------

        private void OnExit(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Tax = Convert.ToDouble(tbTax.Text);
            Properties.Settings.Default.Apr = Convert.ToDouble(tbAPR.Text);
            Properties.Settings.Default.Period = Convert.ToInt32(tbPeriod.Text);
            Properties.Settings.Default.AltPmt = Convert.ToDouble(tbAltPmt.Text);
            Properties.Settings.Default.AltStart = Convert.ToInt32(tbAltStart.Text);
            Properties.Settings.Default.Insurance = Convert.ToDouble(tbInsurance.Text);
            Properties.Settings.Default.LoanAmount = Convert.ToDouble(tbLoanAmount.Text);
            Properties.Settings.Default.DownPayment = Convert.ToDouble(tbDownPayment.Text);

            Properties.Settings.Default.Save();

            Environment.Exit(0);
        }

        // ------------------------------------------------

        private void OnEnter(object sender, RoutedEventArgs e)
        {
            ((TextBox)sender).SelectAll();
        }

        // ------------------------------------------------

        private void OnPrint(object sender, RoutedEventArgs e)
        {
            gridPrinter._pageNumber = 1;
            gridPrinter._rowCount = 0;

            printDocument.Print();
        }


        // ------------------------------------------------

        private void PrintPage(object sender, PrintPageEventArgs e)
        {
            var graphics = e.Graphics;
            var more = gridPrinter.DrawDataGrid(graphics);

            if(more == true)
            {
                e.HasMorePages = true;
                gridPrinter._pageNumber++;
            }
        }
    }
}
