using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BonafideCertificateGenerator
{
    /// <summary>
    /// Interaction logic for BonafideLetterPage.xaml
    /// </summary>
    public partial class BonafideLetterPage : Window
    {
        string rollNo;
        string content;
        string address;
        bool generalBonafide = false;
        public void loadData()
        {
            bodyContentBox.Text = content;
            dateBox.Text = "Dt : " + DateTime.Today.Day + "." + DateTime.Today.Month + "." + DateTime.Today.Year;
            hodAddressBox.Text = (generalBonafide) ? "HOD NAME\nDESHIGNATION\nDepartment\n" : ""  ;
            passportAddressBox.Text = address;
            bonafideCertificateTitleBox.Text = "BONA FIDE CERTIFICATE";
            forPrincipleBox.Text = "for PRINCIPAL";
        }
        public BonafideLetterPage(string content,string rollNo,string address=null,bool generalBonafide = false)
        {
            InitializeComponent();
            this.rollNo = rollNo;
            this.content = content;
            this.address ="\n"+address;
            this.generalBonafide = generalBonafide;
            loadData();
        }
        void generateCertificate()
        {

            var font = FontFactory.GetFont("Aerial", 11.0f, Font.DEFAULTSIZE, BaseColor.BLACK);
            var boldFont = FontFactory.GetFont("Aerial", 11.0f, BaseColor.BLACK);
            var document = new Document(PageSize.A4, 100f, 100f, 75f, 20f);

            var output = new FileStream(@ConfigurationManager.AppSettings["pdfFileLocation"]+rollNo+".pdf", FileMode.Create);
            var writer = PdfWriter.GetInstance(document, output);
          

            iTextSharp.text.Paragraph paradate = new iTextSharp.text.Paragraph();
            paradate.Add("\n\n\n\n" + dateBox.Text);
            paradate.Alignment = Element.ALIGN_RIGHT;
            paradate.Font = font;

            iTextSharp.text.Paragraph paraHodAddress = new iTextSharp.text.Paragraph();
            if (generalBonafide)
            {
                paraHodAddress.Add("\n" + hodAddressBox.Text);
                paraHodAddress.Font = font;
                paraHodAddress.Alignment = Element.ALIGN_LEFT;
            }

            iTextSharp.text.Paragraph paratitle = new iTextSharp.text.Paragraph();
            paratitle.Add("\n\n"+bonafideCertificateTitleBox.Text);
            paratitle.Alignment = Element.ALIGN_CENTER;
            paratitle.Font.SetStyle(Font.BOLD);




            iTextSharp.text.Paragraph paracontent = new iTextSharp.text.Paragraph();
            paracontent.Add("\n\n            " + bodyContentBox.Text);
            paracontent.Font = font;
            paracontent.Alignment = Element.ALIGN_JUSTIFIED;
        
            iTextSharp.text.Paragraph paraddress = new iTextSharp.text.Paragraph();
            if (address == null)
            {

            }
            else
            {
                paraddress.Add("\n            " + passportAddressBox.Text);
                paraddress.Font = font;
                paraddress.IndentationLeft = 120f;
            }



            iTextSharp.text.Paragraph paratanq = new iTextSharp.text.Paragraph();
            paratanq.Add("\n\n"+forPrincipleBox.Text);
            paratanq.Alignment = Element.ALIGN_RIGHT;
            paratanq.Font = font;

            document.Open();
            document.AddTitle("BONAFIDE CERTIFICATE");
            document.Add(paradate);
            document.Add(paraHodAddress);
            document.Add(paratitle);
            document.Add(paracontent);
            document.Add(paraddress);
            document.Add(paratanq);
            document.Close();
        }
        private void editBonafideContentButton_Click(object sender, RoutedEventArgs e)
        {
            if (editBonafideContentButton.Content.ToString() == "EDIT")
            {
                editBonafideContentButton.Content = "DONE";
                generateBonafidePDFButton.IsEnabled = false;
                dateBox.IsEnabled = hodAddressBox.IsEnabled = bodyContentBox.IsEnabled = bonafideCertificateTitleBox.IsEnabled = forPrincipleBox.IsEnabled = passportAddressBox.IsEnabled = true;
            }
            else
            {
                editBonafideContentButton.Content = "EDIT";
                dateBox.IsEnabled = hodAddressBox.IsEnabled = bodyContentBox.IsEnabled = bonafideCertificateTitleBox.IsEnabled = forPrincipleBox.IsEnabled = passportAddressBox.IsEnabled = false;
                generateBonafidePDFButton.IsEnabled = true;
            }
        }

        private async void generateBonafidePDFButton_Click(object sender, RoutedEventArgs e)
        {
            generateBonafidePDFButton.IsEnabled = false;
            await Task.Yield();
            generateCertificate();
            MessageBox.Show("CREATING PDF\nPLEASE WAIT...", "PDF", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            generateBonafidePDFButton.IsEnabled = true;
        }
    }
}
