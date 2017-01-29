using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.OleDb;
using System.Data;
using System.Configuration;
using System.Net.Mail;
using System.Net;
using System.Windows.Threading;
using System.Text.RegularExpressions;
using RestSharp;
using System.Globalization;

namespace BonafideCertificateGenerator
{
    
    public partial class MainWindow : Window 
    {
        OleDbDataAdapter dAdapter;
        OleDbCommand cmd;
        string rollNo = string.Empty;
        string tutorId = string.Empty;
        
        BonafideDatabase database;

      
        public MainWindow() 
        {
            InitializeComponent();
            database = new BonafideDatabase();
            database.openConnection();
            adminPanel.Visibility = Visibility.Visible;
            studentFormGrid.Visibility = Visibility.Visible;
        }
        
        private async void refreshTable_Click(object sender, RoutedEventArgs e)
        {
            refreshTable.IsEnabled = false;
            await Task.Yield();
            database.loadStudentData(showBonafideTable,adminRollNoSelectionBox,1);
            refreshTable.IsEnabled = true;
        }

        private void printButton_Click(object sender, RoutedEventArgs e)
        {
            printButton.IsEnabled = false;

            string internshipContent = string.Empty;
            string content = string.Empty;
            string address = string.Empty;
            bool generalBonafide = false;
            if (adminRollNoSelectionBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a Roll no", "NO SELECTION MADE", MessageBoxButton.OK, MessageBoxImage.Error);
                adminRollNoSelectionBox.Focus();
                return;
            }
            else
            {
                
                if (adminReasonSelectionBox.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select a Request of the RollNo" + tutorRollnoSelectionBox.SelectedItem.ToString(), "NO REQUEST SELECTED", MessageBoxButton.OK, MessageBoxImage.Error);
                    adminReasonSelectionBox.Focus();
                    return;
                }
                else { 
                    DataTable getStudentRecord = new DataTable();
                    cmd = new OleDbCommand("Select * from StudentDetails where RollNo = @rollNo");
                    cmd.Parameters.AddWithValue("@rollNo", adminRollNoSelectionBox.Text);
                    dAdapter = database.executeDataAdapter(cmd);
                    dAdapter.Fill(getStudentRecord);
                   
                    if (adminReasonSelectionBox.SelectedItem.ToString() == "For Passport")
                    {
                        content = "     This is to certify that " + (((getStudentRecord.Rows[0][6]).ToString() == "Male") ? "Mr." : "Mrs.") + " " + getStudentRecord.Rows[0][1] + " (" + getStudentRecord.Rows[0][0] + ") " + (((getStudentRecord.Rows[0][6]).ToString() == "Male") ? "S/o" : "D/o") + " " + getStudentRecord.Rows[0][10] + " is a bonafide student of COLLEGE NAME, colege location doing " + (((getStudentRecord.Rows[0][6]).ToString() == "Male") ? "his " : "her ") + getStudentRecord.Rows[0][3] + " " + getStudentRecord.Rows[0][4] + " Programme." + (((getStudentRecord.Rows[0][6]).ToString() == "Male") ? " His " : " Her ") + "date of birth is " + getStudentRecord.Rows[0][8].ToString().Replace('/', '.') + ". " + (((getStudentRecord.Rows[0][6]).ToString() == "Male") ? "He" : "She") + " holds an Indian Citizenship. This certificate is issued for the purpose of applying for Passport. " + (((getStudentRecord.Rows[0][6]).ToString() == "Male") ? "He " : "She ") + "is residing at ";
                        address = getStudentRecord.Rows[0][7].ToString();
                    }
                    else if(adminReasonSelectionBox.SelectedItem.ToString() == "For Bank Account")
                    {
                        content = "     This is to certify that " + (((getStudentRecord.Rows[0][6]).ToString() == "Male") ? "Mr." : "Mrs.") + " " + getStudentRecord.Rows[0][1] + " (" + getStudentRecord.Rows[0][0] + ")" + " is a bonafide student of COLLEGE NAME, colege location doing " + (((getStudentRecord.Rows[0][6]).ToString() == "Male") ? "his " : "her ") + getStudentRecord.Rows[0][3] + " " + getStudentRecord.Rows[0][4] + " Programme." + " This certificate is issued for the purpose of opening a Bank Account.";
                    }
                    else if(adminReasonSelectionBox.SelectedItem.ToString() == "For Educational Loan")
                    {
                        content = "     This is to certify that " + (((getStudentRecord.Rows[0][6]).ToString() == "Male") ? "Mr." : "Mrs.") + " " + getStudentRecord.Rows[0][1] + " (" + getStudentRecord.Rows[0][0] + ")" + " is a bonafide student of COLLEGE NAME, colege location doing " + (((getStudentRecord.Rows[0][6]).ToString() == "Male") ? "his " : "her ") + getStudentRecord.Rows[0][3] + " " + getStudentRecord.Rows[0][4] + " Programme." + " This certificate is issued for the purpose of applying for Educational Loan.";
                    }
                    else if(adminReasonSelectionBox.SelectedItem.ToString() == "For Scholarship")
                    {
                        content = "     This is to certify that " + (((getStudentRecord.Rows[0][6]).ToString() == "Male") ? "Mr." : "Mrs.") + " " + getStudentRecord.Rows[0][1] + " (" + getStudentRecord.Rows[0][0] + ")" + " is a bonafide student of COLLEGE NAME, colege location doing " + (((getStudentRecord.Rows[0][6]).ToString() == "Male") ? "his " : "her ") + getStudentRecord.Rows[0][3] + " " + getStudentRecord.Rows[0][4] + " Programme." + " This certificate is issued for the purpose of applying for Scholarship.";
                      }
                    else if (adminReasonSelectionBox.SelectedItem.ToString() == "For General Purpose")
                    {
                        content = "     This is to certify that " + (((getStudentRecord.Rows[0][6]).ToString() == "Male") ? "Mr." : "Mrs.") + " " + getStudentRecord.Rows[0][1] + " (" + getStudentRecord.Rows[0][0] + ")" + " is a bonafide student of COLLEGE NAME, colege location doing " + (((getStudentRecord.Rows[0][6]).ToString() == "Male") ? "his " : "her ") + getStudentRecord.Rows[0][3] + " " + getStudentRecord.Rows[0][4] + " Programme." + "";
                        generalBonafide = true;
                    }
                    else if(adminReasonSelectionBox.SelectedItem.ToString() == "For Internship")
                    {
                        string[] monthInWords = { "", "One ", "Two", "Three ", "Four ", "Five ", "Six ", "Seven ", "Eight ", "Nine" , "Ten", "Eleven", "Twelve" };
                        string[] semInRomanLetter = { "", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X" };
                        content = "This is to certify that " + (((getStudentRecord.Rows[0][6]).ToString() == "Male") ? "Mr." : "Mrs.") + " " + getStudentRecord.Rows[0][1] + " (" + getStudentRecord.Rows[0][0] + ")" + " is a bonafide student of COLLEGE NAME, colege location doing " + (((getStudentRecord.Rows[0][6]).ToString() == "Male") ? "his " : "her ") + getStudentRecord.Rows[0][3] + " " + getStudentRecord.Rows[0][4] + " Programme.";
                        cmd = new OleDbCommand("select * from InternshipCertificateTable where RollNo = @rollNo");
                        cmd.Parameters.AddWithValue("@rollNo", adminRollNoSelectionBox.Text);
                        OleDbDataAdapter getIntern = database.executeDataAdapter(cmd);
                        DataTable getInternData = new DataTable();
                        getIntern.Fill(getInternData);
                        string sDate = getInternData.Rows[0][2].ToString();
                        string eDate = getInternData.Rows[0][3].ToString();
                        IFormatProvider culture = new System.Globalization.CultureInfo("fr-FR", true);
                        internshipContent = "The  student  is  required  to  undertake  a  live  project  for  "+ (monthInWords[Math.Abs ( (Convert.ToDateTime(eDate, culture).Month - Convert.ToDateTime(sDate, culture).Month) + 12 * (Convert.ToDateTime(eDate, culture).Year - Convert.ToDateTime(sDate, culture).Year) )]) + "  months during  the "+ semInRomanLetter[(((Math.Abs(int.Parse(DateTime.Now.ToString("yy")) - int.Parse(adminRollNoSelectionBox.SelectedItem.ToString().Remove(2))) * 12 )- 6)/ 6)+1]+" semester and submit a project report as a part of the curriculum. In this connection, I request you to consider favorably  case, for allotting a suitable project and permitting "+ (((getStudentRecord.Rows[0][6]).ToString() == "Male") ? "his " : "her ")+ "to work on it in your esteemed organization from "+ (Convert.ToDateTime(sDate, culture).ToString("MMMM") + " " + (Convert.ToDateTime(sDate, culture).ToString("yyyy"))) + " to "+ (Convert.ToDateTime(eDate, culture).ToString("MMMM") +" "+(Convert.ToDateTime(eDate,culture).ToString("yyyy")))+ ".\n\n";
                        address = getInternData.Rows[0][4].ToString();

                    }
                    
                    
                }
            }
            MessageBox.Show("GENERATING CERTIFICATE :" + adminRollNoSelectionBox.SelectedItem.ToString() + " : " + adminReasonSelectionBox.SelectedItem.ToString() + "", "GENERATING...PLEASE WAIT", MessageBoxButton.OK, MessageBoxImage.Information);
            var page = new Window(); 
            if (adminReasonSelectionBox.SelectedItem.ToString() == "For Internship")
            {
                page = new CertificatePage(content,adminRollNoSelectionBox.SelectedItem.ToString(),internshipContent,address);
            }
            else
            {
                page = new BonafideLetterPage(content, adminRollNoSelectionBox.SelectedItem.ToString(), address, generalBonafide);
            }
            if (MessageBoxResult.Yes == MessageBox.Show("Do you want to create a pdf ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Exclamation))
            {
                new AutoMail(database.getStudentMailId(adminRollNoSelectionBox.SelectedItem.ToString()), adminReasonSelectionBox.SelectedItem.ToString(), database.getStudentName(adminRollNoSelectionBox.SelectedItem.ToString()), 2).sendMail();
                page.Show();
                cmd = new OleDbCommand("Update CertificateTable set status = 2 where RollNo = @rollNo and Reason = @reason");
                cmd.Parameters.AddWithValue("@reason", adminReasonSelectionBox.Text);
                cmd.Parameters.AddWithValue("@rollNo", adminRollNoSelectionBox.Text);
                database.executeSql(cmd);
                if (defaultReasonBox.SelectionBoxItem.ToString() == "For Internship")
                {
                    cmd = new OleDbCommand("Delete from InternshipCertificateTable Where RollNo = @rollNo");
                    cmd.Parameters.AddWithValue("@rollNo", adminRollNoSelectionBox.Text);
                    database.executeSql(cmd);
                    cmd = new OleDbCommand("Delete from CertificateTable where RollNo = @rollNo and Reason = @reason");
                    cmd.Parameters.AddWithValue("@rollNo", adminRollNoSelectionBox.Text);
                    cmd.Parameters.AddWithValue("@reason", adminReasonSelectionBox.Text);
                    database.executeSql(cmd);            
                }
                else
                {
                    cmd = new OleDbCommand("Delete from CertificateTable where RollNo = @rollNo and Reason = @reason");
                    cmd.Parameters.AddWithValue("@rollNo", adminRollNoSelectionBox.Text);
                    cmd.Parameters.AddWithValue("@reason", adminReasonSelectionBox.Text);
                    database.executeSql(cmd);
                }
                
                if (database.checkInternshipTable(adminRollNoSelectionBox.Text) == 0 && database.checkStudentCertificateTable(adminRollNoSelectionBox.Text,adminReasonSelectionBox.Text) == 0)
                {
                    cmd = new OleDbCommand("Delete from StudentDetails where RollNo = @rollNo");
                    cmd.Parameters.AddWithValue("@rollNo", adminRollNoSelectionBox.Text);
                    database.executeSql(cmd);
                }
            }
            else
            {
                return;
            }
            database.loadStudentData(showBonafideTable, adminRollNoSelectionBox, 1);
            printButton.IsEnabled = true;
        }

       
        public bool IsValid(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
        static bool IsValidMailAddress1(string mail)
        {
            try
            {
                System.Net.Mail.MailAddress mailAddress = new System.Net.Mail.MailAddress(mail);

                return true;
            }
            catch
            {
                return false;
            }
        }

 
        private async void StudentBackButton_Click(object sender, RoutedEventArgs e)
        {
            StudentBackButton.IsEnabled = false;
            await Task.Yield();
            loginUserNameBox.Text = loginPasswordBox.Password = "";
            firstGrid.Visibility = Visibility.Visible;
            StudentBackButton.IsEnabled = true;
        }
        private void submitStudentForm(object sender, RoutedEventArgs e)
        {
            if (userFirstNameBox.Text == "" || userSecondNameBox.Text == "")
            {
                MessageBox.Show("Please Fill Your First Name and Second Name", "Name", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (fathersFirstNameBox.Text == "" || fathersSecondNameBox.Text == "")
            {
                MessageBox.Show("Please Fill Fathers First Name and Second Name", "Name", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!Regex.Match(userFirstNameBox.Text, "^[A-Z][a-zA-Z]*$").Success)
            {
                MessageBox.Show("Invalid first name", "Message", MessageBoxButton.OK, MessageBoxImage.Error);
                userFirstNameBox.Focus();
                return;
            }

            if (!Regex.Match(userSecondNameBox.Text, "^[A-Z][a-zA-Z]*[[a-zA-Z]*|.| ]+$").Success)
            {
                MessageBox.Show("Invalid Second name", "Message", MessageBoxButton.OK, MessageBoxImage.Error);
                userSecondNameBox.Focus();
                return;
            }

            if (!Regex.Match(fathersFirstNameBox.Text, "^[A-Z][a-zA-Z]*$").Success)
            {
                MessageBox.Show("Invalid First name", "Message", MessageBoxButton.OK, MessageBoxImage.Error);
                fathersFirstNameBox.Focus();
                return;
            }
            if (!Regex.Match(fathersSecondNameBox.Text, "^[A-Z][a-zA-Z]*$").Success)
            {
                MessageBox.Show("Invalid Second name", "Message", MessageBoxButton.OK, MessageBoxImage.Error);
                fathersSecondNameBox.Focus();
                return;
            }

            if (!Regex.Match(rollNoTextBox.Text, "^[1-9][0-9][a-zA-Z][a-zA-Z][0-9][0-9]$").Success)
            {
                MessageBox.Show("Invalid Roll No", "Message", MessageBoxButton.OK, MessageBoxImage.Error);
                rollNoTextBox.Focus();
                return;
            }
            if (!IsValidMailAddress1(emailBox.Text))
            {
                MessageBox.Show("****INVALID EMAIL ADDRESS****", "EMAIL ERROR");
                return;
            }
            if (permanentAddressBox.Text == null)
            {
                MessageBox.Show("Please Enter your address", "ADDRESS NOT FILLED", MessageBoxButton.OK, MessageBoxImage.Information);
                permanentAddressBox.Focus();
                return;
            }
            else
            {
                if (Regex.Match(permanentAddressBox.Text, "[!~`@#$%^*_+={}\\|?;:\\[\\]]").Success)
                {
                    MessageBox.Show("Permanent addres box error");
                    permanentAddressBox.Focus();
                    return;
                }
            }
            if (defaultReasonBox.SelectedIndex == -1)
            {
                MessageBox.Show("Reason for Bonafide not Selected");
                defaultReasonBox.Focus();
                return;
            }
            MessageBox.Show(dateOfBirthBox.SelectedDate + "" + defaultReasonBox.SelectedItem + "" + permanentAddressBox.Text);
            if (dateOfBirthBox.SelectedDate == null)
            {
                MessageBox.Show("Please select your Date of Birth", "NO DOB SELECTED", MessageBoxButton.OK, MessageBoxImage.Information);
                dateOfBirthBox.Focus();
                return;

            }
            else
            {
                if (DateTime.Today.Year - dateOfBirthBox.SelectedDate.Value.Year <= 17)
                {
                    MessageBox.Show("It seems like you have entered fake date of birth. Please enter your own Date of birth", "DATE OF BIRTH ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    dateOfBirthBox.Focus();
                    return;
                }

            }
            if (internshipDetailsTab.IsVisible == true)
            {

                if (!(iStartDate.SelectedDate.Value.CompareTo(iEndDate.SelectedDate.Value) < 0))
                {
                    MessageBox.Show("Start Date should be less than end date", "Start date and end date Error", MessageBoxButton.OK, MessageBoxImage.Information);
                    iEndDate.Focus();
                    return;
                }
                if (companyAddressBox.Text == null)
                {
                    MessageBox.Show("Please Enter your address", "ADDRESS NOT FILLED", MessageBoxButton.OK, MessageBoxImage.Information);
                    companyAddressBox.Focus();
                    return;
                }
                else
                {
                    if (Regex.Match(companyAddressBox.Text, "[!~`@#$%^*_+={}\\|?;\\[\\]]").Success)
                    {
                        MessageBox.Show("Company/University address box filled with invalid data");
                        companyAddressBox.Focus();
                        return;
                    }
                }
            }

            if (MessageBoxResult.Yes == MessageBox.Show("Confirm Submission", "Confirmation Window", MessageBoxButton.YesNo, MessageBoxImage.Information))
            {
                try
                {
                    tutorId = database.getTutorId(new string(rollNoTextBox.Text.ToString().ToCharArray(0, 4).ToArray()));
                    if ((database.checkStudentTable(rollNoTextBox.Text) == 0))
                    {
                        cmd = new OleDbCommand("INSERT INTO StudentDetails(RollNo,StudentName,Department,CurrentYear,Course,EmailId,Gender,DateOfBirth,Address,TutorId,FatherName) VALUES (@rollNo,@studentName,@department,@currentYear,@course,@emailId,@gender,@dob,@address,@tutorId,@fathername)");
                        cmd.Parameters.AddWithValue("@rollNo", rollNoTextBox.Text);
                        cmd.Parameters.AddWithValue("@studentName", userFirstNameBox.Text + " " + userSecondNameBox.Text);
                        cmd.Parameters.AddWithValue("@department", departmentBox.Text);
                        cmd.Parameters.AddWithValue("@currentYear", currentYearBox.Text);
                        cmd.Parameters.AddWithValue("@course", courseBox.Text);
                        cmd.Parameters.AddWithValue("@emailId", emailBox.Text);
                        cmd.Parameters.AddWithValue("@gender", genderBox.Text);
                        cmd.Parameters.AddWithValue("@dob", dateOfBirthBox.SelectedDate.Value.ToShortDateString());
                        cmd.Parameters.AddWithValue("@address", permanentAddressBox.Text);
                        cmd.Parameters.AddWithValue("@tutorId", tutorId);
                        cmd.Parameters.AddWithValue("@fatherName", fathersFirstNameBox.Text + " " + fathersSecondNameBox.Text);
                        database.executeSql(cmd);
                        cmd.Dispose();
                    }
                    if (database.checkStudentCertificateTable(rollNoTextBox.Text, defaultReasonBox.Text) == 0)
                    {
                        cmd = new OleDbCommand("INSERT INTO CertificateLogDetails (RollNo,Reason,StudentName) values(@rollNo,@reason,@name)");
                        cmd.Parameters.AddWithValue("@rollNo", rollNoTextBox.Text);
                        cmd.Parameters.AddWithValue("@reason", defaultReasonBox.Text);
                        cmd.Parameters.AddWithValue("@name", userFirstNameBox.Text + " " + userSecondNameBox.Text);
                        database.executeSql(cmd);
                        cmd.Dispose();

                        if (internshipDetailsTab.IsVisible == true)
                        {
                            cmd = new OleDbCommand("INSERT INTO CertificateTable (RollNo, Reason, Status) values(@rollNo,@reason,0)");
                            cmd.Parameters.AddWithValue("@rollNo", rollNoTextBox.Text);
                            cmd.Parameters.AddWithValue("@reason", defaultReasonBox.Text);
                            database.executeSql(cmd);
                            cmd.Dispose();
                            cmd = new OleDbCommand("INSERT INTO InternshipCertificateTable (RollNo,Reason,StartPeriod,EndPeriod,CompanyAddress) values (@rollNo,@reason,@startDate,@endDate,@companyAddress)");
                            cmd.Parameters.AddWithValue("@rollNo", rollNoTextBox.Text);
                            cmd.Parameters.AddWithValue("@reason", defaultReasonBox.Text);
                            cmd.Parameters.AddWithValue("@startDate", iStartDate.SelectedDate.Value.ToShortDateString());
                            cmd.Parameters.AddWithValue("@endDate", iStartDate.SelectedDate.Value.ToShortDateString());
                            cmd.Parameters.AddWithValue("@companyAddress", companyAddressBox.Text);
                            database.executeSql(cmd);
                            cmd.Dispose();
                        }
                        else
                        {
                            cmd = new OleDbCommand("INSERT INTO CertificateTable (RollNo, Reason, Status) values(@rollNo,@reason,0)");
                            cmd.Parameters.AddWithValue("@rollNo", rollNoTextBox.Text);
                            cmd.Parameters.AddWithValue("@reason", defaultReasonBox.Text);
                            database.executeSql(cmd);
                        }
                    }
                    else
                    {

                        MessageBox.Show("Already Applied", "Duplication Error");
                        defaultReasonBox.Focus();
                        return;
                    }
                    new AutoMail(database.getStudentMailId(rollNoTextBox.Text), defaultReasonBox.SelectionBoxItem.ToString(), database.getStudentName(rollNoTextBox.Text), 0).sendMail();
                    if (!new AutoMail(database.getTutorMailId(database.getTutorId(new string(rollNoTextBox.Text.ToString().ToCharArray(0, 4).ToArray()))), defaultReasonBox.SelectionBoxItem.ToString(), database.getStudentName(rollNoTextBox.Text), 4).sendMail())
                    {
                        MessageBox.Show("Submission Success", "No Internet Connection", MessageBoxButton.OK);
                    }
                    MessageBox.Show("Shortly you will be notified to your given mail address about the status", "SUBMISSION SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception dbError)
                {
                    MessageBox.Show("Submission Not successfull" + dbError.ToString() + "", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                applyForBonafideButton.Focus();
                return;
            }

        }


        private async void loginButton_Click(object sender, RoutedEventArgs e)
        {
            loginButton.IsEnabled = false;
            await Task.Yield();
            if (loginUserNameBox.Text == "admin" && loginPasswordBox.Password == "admin")
            {
                adminPanel.Visibility = Visibility.Visible;
                loginPanel.Visibility = Visibility.Hidden;
           
                {
           //         database.loadStudentData(showBonafideTable, adminRollNoSelectionBox, 1);
                }
            }
            else if (loginUserNameBox.Text == "user" && loginPasswordBox.Password == "user")
            {
                adminPanel.Visibility = Visibility.Visible;
                loginPanel.Visibility = Visibility.Hidden;
                database.loadStudentData(showBonafideTable, adminRollNoSelectionBox, 1);
            }
            else if (loginUserNameBox.Text == "14S21" && loginPasswordBox.Password == "staff")
            {
                tutorApprovalPanel.Visibility = Visibility.Visible;
                loginPanel.Visibility = studentFormGrid.Visibility = Visibility.Hidden;
                cmd = new OleDbCommand("Select * from TutorDetails Where TutorId = @tutorId");
                cmd.Parameters.AddWithValue("@tutorId", loginUserNameBox.Text);
                var recordValue = database.executeReader(cmd);
                if (recordValue.Read())
                {
                    database.loadStudentData(showTutorClassStudentsDataGrid, tutorRollnoSelectionBox, 0, new string((recordValue["CourseCode"].ToString()).Where(char.IsLetter).ToArray()));
                }
                else
                {
                    MessageBox.Show("Tutor Details Not yet updated.Please ask the admin to update your details in the database", "DETAILS NOT AVAILABLE", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                }
            else if (selectStaff.IsChecked == true )
            {

                //use scrapping code from college website for checking valid tutor
            }
            else if (selectStudent.IsChecked == true)
            {
                //use scrapping code from college website for checking valid student 
			}
            else
            {
                MessageBox.Show("USER NAME / PASSWORD ERROR", "INVALID ENTRY");
            }
            loginUserNameBox.Clear();
            loginPasswordBox.Clear();
            loginButton.IsEnabled = true;
        }

        private async void checkStatusButton_Click(object sender, RoutedEventArgs e)
        {
            checkStatusButton.IsEnabled = false;
            await Task.Yield();
            checkStatusLabel.Visibility = Visibility.Visible;
            checkRollNoInDbBox.Visibility = Visibility.Visible;
            checkRollNoStatusButton.Visibility = Visibility.Visible;  
            checkStatusButton.IsEnabled = true;
        }

       
        
        private async void checkRollNoStatusButton_Click(object sender, RoutedEventArgs e)
        {
            checkRollNoStatusButton.IsEnabled = false;
           
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 25);
            showStatusContent.Content = database.getStatusFromDb(checkRollNoInDbBox.Text);
            
            if (showStatusContent.Content.ToString().Length > 18)
            {
                showStatusContent.Visibility = Visibility.Visible;      
                timer.Tick += timer_Tick;                
                timer.Start();
            }
            else
            {
                showStatusContent.Visibility = Visibility.Visible;
                showStatusContent.Content = "ERROR : INVALID ROLL NO/ NOT IN DATABASE / CHECK YOUR MAIL";               
            }
            await Task.Yield();
            checkRollNoStatusButton.IsEnabled = true;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            showStatusContent.Visibility = Visibility.Hidden;
            checkRollNoInDbBox.Text = "";
        }

        private async void applyForBonafideButton_Click(object sender, RoutedEventArgs e)
        {
            applyForBonafideButton.IsEnabled = false;
            await Task.Yield();
            firstGrid.Visibility = Visibility.Hidden;
            adminPanel.Visibility =Visibility.Hidden;
            loginPanel.Visibility = Visibility.Visible;
            studentFormGrid.Visibility = Visibility.Hidden;
            tutorApprovalPanel.Visibility = tutorClassSelectionPanel.Visibility = Visibility.Hidden;
            applyForBonafideButton.IsEnabled = true;
        }

        private async void adminButton_Click(object sender, RoutedEventArgs e)
        {
            adminButton.IsEnabled = false;
            await Task.Yield();
            firstGrid.Visibility = Visibility.Hidden;
            studentFormGrid.Visibility = Visibility.Hidden;
            adminPanel.Visibility = tutorApprovalPanel.Visibility = tutorClassSelectionPanel.Visibility = Visibility.Hidden;
            loginPanel.Visibility = Visibility.Visible;
            adminButton.IsEnabled = true;

        }

        private async void loginBackButton_Click(object sender, RoutedEventArgs e)
        {
            loginBackButton.IsEnabled = false;
            await Task.Yield();

            loginUserNameBox.Text = loginPasswordBox.Password = "";
            firstGrid.Visibility = Visibility.Visible;
            studentFormGrid.Visibility = Visibility.Hidden;
            loginPanel.Visibility = Visibility.Hidden;
            adminPanel.Visibility = Visibility.Hidden;
            loginBackButton.IsEnabled = true;
        }

        private async void tutorSelectionProceedButton_Click(object sender, RoutedEventArgs e)
        {
            tutorSelectionProceedButton.IsEnabled = false;
            await Task.Yield();
            tutorApprovalPanel.Visibility = Visibility.Visible;
            tutorClassSelectionPanel.Visibility = Visibility.Hidden;
            firstGrid.Visibility = Visibility.Hidden;
            studentFormGrid.Visibility = Visibility.Hidden;
            adminPanel.Visibility = Visibility.Hidden;
            loginPanel.Visibility = Visibility.Hidden;
            tutorSelectionProceedButton.IsEnabled = true;
        }

        private async void tutorSecondPanelBackButton_Click(object sender, RoutedEventArgs e)
        {
            tutorApprovalButton.IsEnabled = false;
            await Task.Yield();
            tutorApprovalPanel.Visibility = Visibility.Hidden;
            loginPanel.Visibility = Visibility.Visible;
            tutorApprovalButton.IsEnabled = true;
        }

        private async void tutorApprovalButton_Click(object sender, RoutedEventArgs e)
        {
            tutorApprovalButton.IsEnabled = false;
            if (tutorRollnoSelectionBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a Roll no", "NO SELECTION MADE", MessageBoxButton.OK, MessageBoxImage.Error);
                tutorRollnoSelectionBox.Focus();
                return;
            }
            else
            {
                if (tutorSelectedReasonBox.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select a Request of the RollNo" + tutorRollnoSelectionBox.SelectedItem.ToString(), "NO REQUEST SELECTED", MessageBoxButton.OK, MessageBoxImage.Error);
                    tutorSelectedReasonBox.Focus();
                    return;
                }
                else
                {
                    cmd = new OleDbCommand("Update CertificateTable set status = 1 where RollNo = @tutorRollNo and Reason = @tutorReason");
                    cmd.Parameters.AddWithValue("@tutorRollNo", tutorRollnoSelectionBox.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@tutorReason", tutorSelectedReasonBox.Text.ToString());
                    database.executeSql(cmd);
                    }
            }
            MessageBox.Show("Approval done for " + tutorRollnoSelectionBox.SelectedItem.ToString() + " : " + tutorSelectedReasonBox.SelectedItem.ToString() + "", "APPROVED SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
            await Task.Yield();
            MessageBox.Show("Sending Mail...Please wait", "Mail", MessageBoxButton.OK, MessageBoxImage.Information);
            if (new AutoMail(database.getStudentMailId(tutorRollnoSelectionBox.SelectedItem.ToString()), tutorSelectedReasonBox.SelectedItem.ToString(), "Abbas", 1).sendMail()) {
                MessageBox.Show("Mail Sent Successfully","Success",MessageBoxButton.OK,MessageBoxImage.Information);
            }
            else {
                MessageBox.Show("Mail Not Sent", "No Internet Connection",MessageBoxButton.OK,MessageBoxImage.Information);
            }
            database.loadStudentData(showTutorClassStudentsDataGrid, tutorRollnoSelectionBox, 0);
            tutorApprovalButton.IsEnabled = true;

        }

        private async void tutorRejectionButton_Click(object sender, RoutedEventArgs e)
        {
            tutorRejectionButton.IsEnabled = false;
            await Task.Yield();
            if (MessageBoxResult.Yes == MessageBox.Show("Rejecting a student will delete his/her request from the database and the student will be notified about this in mail","Confirm",MessageBoxButton.YesNo,MessageBoxImage.Information))
            {
                if (database.checkInternshipTable(tutorRollnoSelectionBox.Text) > 0)
                {
                    cmd = new OleDbCommand("DELETE FROM InternshipCertificateTable WHERE (RollNo = @rollNo)");
                    cmd.Parameters.AddWithValue("@rollNo", tutorRollnoSelectionBox.Text);
                    database.executeSql(cmd);
                }
                cmd = new OleDbCommand("DELETE FROM CertificateTable WHERE (RollNo = @rollNo) and (Reason = @reason)");
                cmd.Parameters.AddWithValue("@rollNo", tutorRollnoSelectionBox.Text);
                cmd.Parameters.AddWithValue("@reason", tutorSelectedReasonBox.Text);
                database.executeSql(cmd);
                new AutoMail(database.getStudentMailId(tutorRollnoSelectionBox.SelectedItem.ToString()), tutorSelectedReasonBox.SelectedItem.ToString(), database.getStudentName(tutorRollnoSelectionBox.SelectedItem.ToString()), 3);
                database.loadStudentData(showTutorClassStudentsDataGrid, tutorRollnoSelectionBox, 0);
            }
            tutorRejectionButton.IsEnabled = true;
        }

        private async void defaultReasonBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await Task.Yield();
            if (defaultReasonBox.SelectionBoxItem.ToString() == "For Internship")
            {
                internshipDetailsTab.Visibility = Visibility.Visible;
                internshipDetailsTab.Focus();
            }
            else
            {
                internshipDetailsTab.Visibility = Visibility.Hidden;
            }
        }

        private void rollNoTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void adminRollNoSelectionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
            if (adminRollNoSelectionBox.SelectedIndex != -1)
            {

                database.loadReasonBox(adminRollNoSelectionBox.SelectedItem.ToString(), adminReasonSelectionBox);   
            }
        }

        private void tutorRollnoSelectionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tutorRollnoSelectionBox.SelectedIndex != -1)
            {
               
                database.loadReasonBox(tutorRollnoSelectionBox.SelectedItem.ToString(), tutorSelectedReasonBox);
              
            }
        }

        private async void adminBackButton_Click(object sender, RoutedEventArgs e)
        {
            adminBackButton.IsEnabled = false;
            await Task.Yield();
            loginUserNameBox.Text = loginPasswordBox.Password = "";
            loginPanel.Visibility = Visibility.Visible;
            adminPanel.Visibility = tutorApprovalPanel.Visibility = studentFormGrid.Visibility = Visibility.Hidden;
            adminBackButton.IsEnabled = true;
        }

        private async void showGeneratedListButton_Click(object sender, RoutedEventArgs e)
        {
            showGeneratedListButton.IsEnabled = false;
            await Task.Yield();
            database.loadStudentData(showBonafideTable, adminRollNoSelectionBox, 7);
            showGeneratedListButton.IsEnabled = true;
        }

       
        private void changeDatabasePathButton_Click(object sender, RoutedEventArgs e)
        {
            changeDatabasePathButton.IsEnabled = false;
            if (changeDatabasePathButton.Content.ToString() == "EDIT")
            {
                databasePathLocation.IsEnabled = true;
                changeDatabasePathButton.Content = "SAVE/TEST";
            }
            else
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                config.AppSettings.Settings["databaseLocation"].Value = databasePathLocation.Text;
                config.Save();
                ConfigurationManager.RefreshSection("appSettings");

                database = new BonafideDatabase();
                if (database.openConnection())
                {
                    MessageBox.Show("Connected Successfully");
                }
                else
                {
                    MessageBox.Show("Not Connected");
                }
                databasePathLocation.IsEnabled = false;
                changeDatabasePathButton.Content = "EDIT";
            }
            changeDatabasePathButton.IsEnabled = true;
        }

        private void adminMailCerdentials_Click(object sender, RoutedEventArgs e)
        {
            adminMailCerdentials.IsEnabled = false;
            if (adminMailCerdentials.Content.ToString() == "SAVE")
            {
                adminMailCerdentials.Content = "EDIT";
                adminEmailBox.IsEnabled = false;
                adminEmailPassword.IsEnabled = false;
            }
            else 
            {
                adminMailCerdentials.Content = "SAVE";
                adminEmailBox.IsEnabled = true;
                adminEmailPassword.IsEnabled = true;
               }
            adminMailCerdentials.IsEnabled = true;
        }

        private async void adminSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            adminSettingsButton.IsEnabled = false;
            await Task.Yield();
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (adminSettingsButton.Content.ToString() == "SETTINGS")
            {
                controllerPanel.Visibility = Visibility.Visible;
                adminSettingsButton.Content = "DONE";
                adminEmailBox.Text = ConfigurationManager.AppSettings["SenderEmailId"];
                adminEmailPassword.Text = ConfigurationManager.AppSettings["SenderPassword"];
                databasePathLocation.Text = ConfigurationManager.AppSettings["databaseLocation"];
            }
            else
            {
                adminSettingsButton.Content = "SETTINGS";
                controllerPanel.Visibility = Visibility.Hidden;
                config.AppSettings.Settings["SenderEmailId"].Value = adminEmailBox.Text;
                config.AppSettings.Settings["SenderPassword"].Value = adminEmailPassword.Text;
                config.AppSettings.Settings["databaseLocation"].Value = databasePathLocation.Text;
                config.Save();
                ConfigurationManager.RefreshSection("appSettings");

                if (new AutoMail(adminEmailBox + "", "", "", 0).testMail(adminEmailBox.Text, adminEmailPassword.Text))
                {
                    MessageBox.Show("Success", "Account Authenciated", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {

                    MessageBox.Show("Failure", "Account Not Authenciated", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            adminSettingsButton.IsEnabled = true;
        }
     

        protected override void OnClosed(EventArgs e)
        {
            
            base.OnClosed(e);
        }
    }
}
