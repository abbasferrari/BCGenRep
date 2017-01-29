using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.io;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Xml.Linq;
using System.IO;
using System.Data.OleDb;
using System.Configuration;

namespace BonafideCertificateGenerator
{
    /// <summary>
    /// Interaction logic for CertificatePage.xaml
    /// </summary>
    /// 


    public partial class CertificatePage : Window
    {
        string generalContent = string.Empty;
        string rollNo = string.Empty;
        string address = string.Empty;
        string internshipContent = string.Empty;
        void createInternBonafide()
        {
            var font = FontFactory.GetFont("Aerial", 12.0f, BaseColor.BLACK);
            var document = new Document(PageSize.A4, 25, 25, 75, 20);
            var output = new FileStream(@ConfigurationManager.AppSettings["pdfFileLocation"] + rollNo + ".pdf", FileMode.Create);
            var writer = PdfWriter.GetInstance(document, output);


            iTextSharp.text.Paragraph paratitle = new iTextSharp.text.Paragraph();
            paratitle.Font.SetStyle(Font.BOLD);
            paratitle.Add("\n\n\n\n\n" + departmentBox.Text + "");
            paratitle.Alignment = Element.ALIGN_CENTER;



            iTextSharp.text.Paragraph parref = new iTextSharp.text.Paragraph();
            string refText = "\n\n                                                                   " + refTextBox.Text;
            refText = refText + "\n                                                                   " + dateTextBox.Text;
            parref.Font.SetStyle(Font.BOLD);
            parref.Add(refText);
            parref.IndentationLeft = 50f;

            iTextSharp.text.Paragraph paraddresses = new iTextSharp.text.Paragraph();
            string content = "";
            content += "\n\n" + fromAddressTextBox.Text;
            content = content + "To\n" + address + "\n\n\n";
            content = content + sirOrMadamTextBox.Text + "\n\n";
            paraddresses.Add(content);
            paraddresses.IndentationLeft = 50f;
            paraddresses.IndentationRight = 45f;
            paraddresses.Font = font;


            iTextSharp.text.Paragraph paracontent = new iTextSharp.text.Paragraph();
            String bodyContentText = "";
            bodyContentText = bodyContent.Text;
            paracontent.Add(bodyContentText);
            paracontent.IndentationLeft = 50f;
            paracontent.IndentationRight = 45f;
            paracontent.Alignment = Element.ALIGN_JUSTIFIED;
            paracontent.Font = font;

            iTextSharp.text.Paragraph paratanq = new iTextSharp.text.Paragraph();
            string tanqcontent = thankYouBox.Text + "\n\n";
            paratanq.Add(tanqcontent);
            paratanq.Alignment = Element.ALIGN_CENTER;
            paratanq.Font = font;

            iTextSharp.text.Paragraph pararegards = new iTextSharp.text.Paragraph();
            string regardscontent = regardsBox.Text;
            pararegards.Add(regardscontent);
            pararegards.Alignment = Element.ALIGN_RIGHT;
            pararegards.Font = font;
            pararegards.IndentationRight = 45f;


            document.Open();
            document.AddTitle("BONAFIDE CERTIFICATE");
            document.Add(paratitle);
            document.Add(parref);
            document.Add(paraddresses);
            document.Add(paracontent);
            document.Add(paratanq);
            document.Add(pararegards);
            document.Close();





        }
        public void loadDetails()
        {
            departmentBox.Text = "DEPARTMENT NAME//";
            refTextBox.Text = "Ref   : "+ConfigurationManager.AppSettings[new string(rollNo.Where(char.IsLetter).ToArray())]+" / INTERNSHIP / " + DateTime.Now.Year;
            dateTextBox.Text = "" + "Date : " + DateTime.Today.Day + "." + DateTime.Today.Month + "." + DateTime.Today.Year;
            fromAddressTextBox.Text = "HOD NAME\nDESHIGNATION\nDepartment\n\n\n\n";
            toAddressTextBox.Text = address + "";
            sirOrMadamTextBox.Text = "Sir/Madam,";
            bodyContent.Text = "             " + generalContent + "\n\n             " + internshipContent + "\n\n";
            thankYouBox.Text = "Thanking you,";
            regardsBox.Text = "Yours faithfully,\n\n\n(HOD NAME)";
        }
        public CertificatePage(string content, string rollNo, string internshipContent, string address)
        {
            InitializeComponent();
            this.generalContent = content;
            this.rollNo = rollNo;
            this.internshipContent = internshipContent;
            this.address = address;
            loadDetails();

        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            createPDF.IsEnabled = false;
            await Task.Yield();
            try
            {
                createInternBonafide();
                MessageBox.Show("Generating PDF document Please wait...");

            }
            catch
            {
                MessageBox.Show("Error Creating Document. Try Again");
            }
            createPDF.IsEnabled = true;
        }
        private async void editButton_Click(object sender, RoutedEventArgs e)
        {
            editButton.IsEnabled = false;
            await Task.Yield();
            if (editButton.Content.ToString() == "EDIT")
            {
                editButton.Content = "DONE";               
                departmentBox.IsEnabled = sirOrMadamTextBox.IsEnabled =regardsBox.IsEnabled = refTextBox.IsEnabled = dateTextBox.IsEnabled = fromAddressTextBox.IsEnabled = toAddressTextBox.IsEnabled = bodyContent.IsEnabled = thankYouBox.IsEnabled = true;
                createPDF.IsEnabled = false;
            }
            else
            {
                editButton.Content = "EDIT";
                createPDF.IsEnabled = true;
                departmentBox.IsEnabled = sirOrMadamTextBox.IsEnabled = regardsBox.IsEnabled = refTextBox.IsEnabled = dateTextBox.IsEnabled = fromAddressTextBox.IsEnabled = toAddressTextBox.IsEnabled = bodyContent.IsEnabled = thankYouBox.IsEnabled = false;
            }
            editButton.IsEnabled = true;
        }
    }
}
