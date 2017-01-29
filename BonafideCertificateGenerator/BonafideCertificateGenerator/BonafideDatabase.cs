using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BonafideCertificateGenerator
{
    
    public class BonafideDatabase : IDisposable
    {
        OleDbConnection connection;
        OleDbDataAdapter dAdapter;
        DataTable dTable;
        OleDbCommand cmd;
        OleDbDataReader reader;
        public BonafideDatabase()
        {
            connection = new OleDbConnection(@ConfigurationManager.AppSettings["databaseLocation"]);
            getConnected();
        }
        public BonafideDatabase(string dbPath)
        {
            connection = new OleDbConnection("@Provider=Microsoft.Jet.OLEDB.4.0;Data Source="+dbPath);
            getConnected();
        }

        public bool openConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool closeConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool getConnected()
        {
            try
            {
                connection.Open();
                connection.Close();
                return true;
            }
            catch
            {
                MessageBox.Show("DB CONNECTION ERROR");
                return false;
            }
        }
        public string getTutorId(string courseCode)
        {

            cmd = new OleDbCommand("Select * from TutorDetails where CourseCode = @courseCodeValue",connection);
            cmd.Parameters.AddWithValue("@courseCodeValue",courseCode);
            
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return reader["TutorID"].ToString();
            }
            else
            {
                return string.Empty;
            }
        }
        public int checkStudentTable(string rollNo)
        {
          
            cmd = new OleDbCommand("Select count(*) from StudentDetails where RollNo = @rollNo", connection);
            cmd.Parameters.AddWithValue("@rollNo", rollNo);
            return (int)(cmd.ExecuteScalar());
           
        }
        public int checkStudentCertificateTable(string rollNo,string Reason)
        {
            cmd = new OleDbCommand("Select count(*) from CertificateTable Where RollNo = @rollNo and Reason = @reason", connection);
            cmd.Parameters.AddWithValue("@rollNo", rollNo);
            cmd.Parameters.AddWithValue("@reason", Reason);
            return (int)(cmd.ExecuteScalar());
        }
        public int checkInternshipTable(string rollNo)
        {
            cmd = new OleDbCommand("Select count(*) from InternshipCertificateTable where RollNo =  @rollNo",connection);
            cmd.Parameters.AddWithValue("@rollNo", rollNo);
            return (int)(cmd.ExecuteScalar());
        }
        public string getStudentName(string rollNo)
        {
            cmd = new OleDbCommand("Select * from StudentDetails Where RollNo = @rollNo", connection);
            cmd.Parameters.AddWithValue("@rollNo", rollNo);
       
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return reader["StudentName"].ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        public string getStudentMailId(string rollNo)
        {
            cmd = new OleDbCommand("Select * from StudentDetails Where RollNo = @rollNo",connection);
            cmd.Parameters.AddWithValue("@rollNo", rollNo);
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return reader["EmailId"].ToString();
            }
            else
            {
                return string.Empty;
            }
        }
        public string getTutorMailId(string tutorId)
        {
            cmd = new OleDbCommand("Select * from TutorDetails where TutorID = @tutorId", connection);
            cmd.Parameters.AddWithValue("@tutorId", tutorId);
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return reader["TutorMailID"].ToString();
            }
            else
            {
                return string.Empty;
            }
        }
          public string getStatusFromDb(string rollNo)
        {
            string output = string.Empty;
            cmd = new OleDbCommand("Select * from CertificateTable where RollNo = @rollNo", connection);
            cmd.Parameters.AddWithValue("@rollNo", rollNo);
            dAdapter = executeDataAdapter(cmd);
            dTable = new DataTable();
            dAdapter.Fill(dTable);
            output = "YOUR REQUEST FOR :";
            try
            {
                for (int i = 0; i < dTable.Rows.Count; i++)
                {
                    if (dTable.Rows[i][2].ToString() == "0")
                    {
                        output = output + "\n" + dTable.Rows[i][1].ToString() + "- NOT GENERATED YET...PLEASE WAIT";
                    }
                    else if (dTable.Rows[i][2].ToString() == "1")
                    {
                        output = output + "\n" + dTable.Rows[i][1].ToString() + "- APPROVED BY TUTOR...PLEASE WAIT ";
                    }
                    else if (dTable.Rows[i][2].ToString() == "2")
                    {
                        output = output + "\n " + dTable.Rows[i][1].ToString() + "- READY ON TABLE...GET IT NOW ";
                    }
                }
                MessageBox.Show("" + output);
            }
            catch
            {
                MessageBox.Show("" + "ERROR");
            }
            return output;

        }

        public OleDbDataReader executeReader(OleDbCommand command)
        {
            command.Connection = connection;
            OleDbDataReader returnReader = command.ExecuteReader();
            return returnReader;
        }
        public OleDbDataAdapter executeDataAdapter(OleDbCommand command)
        {
            command.Connection = connection;
            return (dAdapter = new OleDbDataAdapter(command));
        }
        public bool executeSql(OleDbCommand command) 
        {

            try
            {
                command.Connection = connection;
                command.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
        }
  
        public void loadStudentData(DataGrid showTable, ComboBox rollNoSelectionBox, int status, string classCourseCode = "")
        {
            rollNoSelectionBox.Items.Clear();
            if (status == 1)     // ADMIN STATUS CODE
            {
                cmd = new OleDbCommand("select * from CertificateTable where Status = @status",connection);
                cmd.Parameters.AddWithValue("@status", status);

            }
            else if(status == 0) // TUTOR STATUS CODE
            {
                cmd = new OleDbCommand("select * from CertificateTable where (RollNo LIKE '%"+@classCourseCode+"%') AND (Status = @status)", connection);
                cmd.Parameters.AddWithValue("@status", status);
                cmd.Parameters.AddWithValue("@classCourseCode", classCourseCode);
            }
            else if (status == 7)   // LOG STATUS CODE
            {
                cmd = new OleDbCommand("select * from CertificateLogDetails", connection);
            }

            dAdapter = executeDataAdapter(cmd);
            dTable = new DataTable();
            dAdapter.Fill(dTable);
            showTable.ColumnWidth = 265;
            showTable.MaxColumnWidth = 265;
            showTable.CanUserAddRows = false;
            showTable.CanUserDeleteRows = false;
            showTable.CanUserResizeColumns = false;
            showTable.CanUserResizeRows = false;
            showTable.CancelEdit();
            showTable.IsReadOnly = true;
            showTable.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            showTable.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            showTable.ItemsSource = dTable.DefaultView;
            for (int i = 0; i < dTable.Rows.Count; i++)
            {
                var item = dTable.Rows[i][0].ToString();
                if (!rollNoSelectionBox.Items.Contains(item))
                {
                    rollNoSelectionBox.Items.Add(dTable.Rows[i][0].ToString());
                }
            }
        }
        public void loadReasonBox(string rollNo, ComboBox reasonBox)
        {
            dTable = null;
            dAdapter = null;
            reasonBox.Items.Clear();
            cmd = new OleDbCommand("select * from CertificateTable where RollNo = @rollNo", connection);
            cmd.Parameters.AddWithValue("@rollNo", rollNo);
            cmd.Connection = connection;
            dAdapter = executeDataAdapter(cmd);
            dTable = new DataTable();
            dAdapter.Fill(dTable);
            for (int i = 0; i < dTable.Rows.Count; i++)
            {
                reasonBox.Items.Add(dTable.Rows[i][1]);
            }
            }

        #region IDisposable Support
        private bool disposedValue = false; 

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    connection.Dispose();
                    dTable.Dispose();
                    dAdapter.Dispose();
                    cmd.Dispose();
                }
                reader = null;            
                disposedValue = true;
            }
        }

         ~BonafideDatabase() {
              Dispose(false);
         }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion


    }
}
