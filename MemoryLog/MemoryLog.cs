/* MemoryLog
 * Author:Smitha Warrier
 * 
 * Aplication to read DS200 log data to DB and generate reports.
 * Version 1.0
  */

using System;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;
using Application = Microsoft.Office.Interop.Excel.Application;
using DataTable = System.Data.DataTable;
using Font = Microsoft.Office.Interop.Excel.Font;
using Label = System.Windows.Forms.Label;
using TextBox = System.Windows.Forms.TextBox;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MemoryLog
{
    public partial class MemoryLog : Form
    {
        private FolderBrowserDialog _fbd;
        private FileInfo[] serialnos;
        DateTime[] creationTimes;
        private DateTime repSdate;
        private DateTime repEdate;
        private DialogResult _result;
        private Color fontColor;
        private BackgroundWorker worker = null;        
        private string _selectedSourcePath;        
        private string _message;
        private string _searchPattern;
        private string precinctFolder;
        private string headerFilePath;
        private string precinct;
        private string[] serialnumber;
        private string election;
        private string precinctId;
        private string precinctIdCode;
        private string serialNo;
       
        private string SerialNumberElectionLog = "";
        private const string connectionString = @"Data Source= localhost\SQLEXPRESS;Initial Catalog=dbo.Memory_Log;User ID=sa;Password=24time";


        //private string[] tablesNames = new[] { "HeaderLog", "SystemLog", "ElectionLog", "CopiedLocations" };
        private string _selectedExcelPath;        
        
        private string repDestn, repElection, repLog;
        private string electionDbName = "";

        private bool IsFirstRowHeader;        
        private bool flagReport;
        private bool repFilter;

        private int copyStep = 0;

        // Excel object references.
        private Application m_objExcel = null;
        private Workbooks m_objBooks = null;
        private _Workbook m_objBook = null;
        private Sheets m_objSheets = null;
        private _Worksheet m_objSheet = null;
        private Range m_objRange = null;
        private Font m_objFont = null;
        private QueryTables m_objQryTables = null;
        private _QueryTable m_objQryTable = null;
        //\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\EDIT//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\

        private List<object> ErrorCode = new List<object>();

        // Frequenty-used variable for optional arguments.
        private object m_objOpt = Missing.Value;
        // Paths used by the sample code for accessing and storing data.
        private object m_strSampleFolder = null;


        public MemoryLog()
        {
            InitializeComponent();
            string strComputerName = Environment.MachineName;
           
            this.startDate.Value = DateTime.Today.AddDays(-7);
            this.endDate.Value = DateTime.Today;

            //Cannot pick dates beyond current date.
            this.startDate.MaxDate = DateTime.Now;
            this.endDate.MaxDate = DateTime.Now;
            EmptyLables();
            FillCombo();
            ReportValidationLabels(true);
            lstPrecinct.Visible = false;
        }

        public MemoryLog(string selectedDestination, string selectedSource)
        {
            _selectedSourcePath = selectedSource;
        }

        //Method to Copy Stick from Source to Destination folder
        private async void btnCopyStick_Click_1(object sender, EventArgs e)
        {
            bool headerExist = CheckForHeaderFile(_selectedSourcePath);
         
            if (headerExist)
            {
                btnCopyStick.Enabled = false;
                _message = @"Copy Action Requested";
                fontColor = Color.CornflowerBlue;
                UpdateLabelStatus(lblStatus, _message, fontColor);
                string copyStatus;
                //Validate Source and Destination selection before trying to copy folders
                if (IsTextValid(txtSource.Text))
                {
                    //Validate directory exists
                    if (!Directory.Exists(txtSource.Text))
                    {
                        lblStatus1.Visible = true;
                        lblStatus1.Text = @"Please check the path. Directory does not exist.";
                    }
                    else
                    {
                        //Before calling DirectoryCopy method,
                        //validate Destination folder. ERROR if file with extension .hdr already exists
                        bool exist = _selectedSourcePath != null && CheckForHeaderFile(_selectedSourcePath);
                        //if folder is clean(NO .hdr file exist) then accept the selected destination folder path
                        if (exist)
                        {
                            SourceBtn.Enabled = false;
                            btnCopyStick.Enabled = false;
                            try
                            {
                                precinct = SearchforHeaderFile(_selectedSourcePath);
                                bool copyFlag = false;
                                //INSERT Data INTO 'CopiedPrecinct' table the Precinct value with CopiedFlag=No
                                if (precinct != null)
                                {
                                    serialnos = SearchforSerialNumber(_selectedSourcePath);
                                    if (serialnos != null)
                                    {
                                        creationTimes = new DateTime[serialnos.Length];
                                        serialnumber = new String[serialnos.Length];
                                        for (int i = 0; i < serialnos.Length; i++)
                                        {
                                            //last file CREATION time.
                                            if (serialnos.Length > i)
                                            {
                                                creationTimes[i] = serialnos[i].CreationTime;
                                                serialNo = serialnos[i].ToString().Split('.').Last();
                                            }
                                            //last file MODIFIED time.
                                            //  if (serialnos.Length > i) creationTimes[i] = serialnos[i].LastWriteTime;
                                            else
                                            {
                                                MessageBox.Show(@"No Files with Serial Number Found.");
                                            }
                                        }
                                        Array.Sort(creationTimes, serialnos);
                                        for (int i = 0; i < serialnos.Length; i++)
                                        {
                                            serialnumber[i] = serialnos[i].ToString();
                                            serialnumber[i].Split('.').Last();
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show(@"No Files with Serial Number Found");
                                    }
                                    _message = @"Copy Action Accepted";
                                    fontColor = Color.Orange;
                                    UpdateLabelStatus(lblStatus, _message, fontColor);
                                    //PICTURE BOX starts here.
                                    pbxBusy.Visible = true;
                                    copyStatus = "0";
                                    copyFlag = false;
                                    SerialNumberElectionLog = getSerialNumber_electionLog(_selectedSourcePath);

         //Check to see if precinct data already exists in the DB before copying data into table
                                    copyFlag = headerFilePath != null &&
                                               UpdateCopiedPrecinctTable(headerFilePath, precinct, copyStatus, SerialNumberElectionLog);

                                    //INSERT .hdr file to 'HEADER table'
                                    if (copyFlag)
                                    {
                                        _message = precinct + @"    Copy  inProgress ";
                                        fontColor = Color.YellowGreen;
                                        UpdateLabelStatus(lblStatus, _message, fontColor);
                                        _message = @" Copying Header Log";
                                        fontColor = Color.CornflowerBlue;
                                        UpdateLabelStatus(lblStatus1, _message, fontColor);
                                        await Task.Delay(2000);
                                        bool headerInsertStatus = InsertHeaderFile(headerFilePath); //Inserted SerialNumber
                                        bool electionInsertStatus = false;
                                        bool systemInsertStatus = false;
                                        bool serialnumberInsertStatus = false;
                                        if (headerInsertStatus)
                                        {
                                            copyStep++;
                                            _message = @"Copying System Log";
                                            fontColor = Color.CornflowerBlue;
                                            UpdateLabelStatus(lblStatus1, _message, fontColor);
                                            //TEST SystemLogInsertFile method
                                            systemInsertStatus = InsertSystemFile(_selectedSourcePath);
                                        }
                                        if (systemInsertStatus)
                                        {
                                            copyStep++;
                                            _message = @" Copying Election Log";
                                            fontColor = Color.CornflowerBlue;
                                            UpdateLabelStatus(lblStatus1, _message, fontColor);
                                            //TEST InsertElectionFile method
                                            electionInsertStatus = InsertElectionFile(_selectedSourcePath);
                                        }
                                        if (electionInsertStatus)
                                        {
                                            //Now insert serialnumber
                                            serialnumberInsertStatus = InsertSerialNumber(election, precinct);
                                            if (!serialnumberInsertStatus)
                                            {
                                                MessageBox.Show(@"No Files with Serial Number Found.");
                                            }
                                            copyStep++;
                                            copyStatus = "1";
                                            copyFlag = false;
                                            copyFlag = headerFilePath != null &&
                                                       UpdateCopiedPrecinctTable(headerFilePath, precinct, copyStatus, SerialNumberElectionLog);
                                            //if UpadteCopiedPrecinctTable complete proceed.
                                            if (copyFlag)
                                            {
                                                copyStep++;
                                                lblStatus1.Text = "";
                                                _message = @"Copy Complete  " + precinct;
                                                fontColor = Color.ForestGreen;
                                                EmptyLables();
                                                UpdateLabelStatus(lblStatus, _message, fontColor);
                                                //empty source text.
                                                //  txtSource.Text = "";
                                                SourceBtn.Enabled = true;
                                                btnCopyStick.Enabled = true;
                                                string electiondb = election;
                                                electiondb = Regex.Replace(electiondb, "[,]", "_");
                                                electiondb = Regex.Replace(electiondb, "[' ']", "");
                                                FillList(election, tableName: "dbo.CopiedLocations_" + electiondb); 
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    lblStatus.Text = "";
                                    lblStatus.Visible = false;
                                }
                                pbxBusy.Visible = false;
                            }
                            catch (Exception ex)
                            {
                                pbxBusy.Visible = false;
                                MessageBox.Show(@"Exception" + ex);
                                SourceBtn.Enabled = true;
                                btnCopyStick.Enabled = true;
                            }
                        }
                    }
                }
                else
                {
                    pbxBusy.Visible = false;
                    //Destination folder not selected.Cannot Proceed.
                    MessageBox.Show(@"Select Source folder to proceed");
                    EmptyLables();
                    //Bring UI foucs on to Source selection Button.
                    SourceBtn.Focus();
                }
            }
            else//if header does not exist
            {
                //set the message to display
                _message = "Header File not found! " + Environment.NewLine + "Select the RIGHT Folder to proceed";
                MessageBox.Show(_message);
                //Wrong Selection. Reset the value of TextBox to empty, to erase any previous selections
                //ClearTextBox(txtSource);
            }

            //Delete partially copied data(unwind update).
            bool cleaningStatus = UnWindUpdateTable(election);
            SourceBtn.Enabled = true;
            btnCopyStick.Enabled = true;
            pbxBusy.Visible = false;
        }

        private void EmptyLables()
        {
            lblStatus.Text = "";
            lblStatus.Visible = false;
            lblStatus.Refresh();
            lblStatus1.Text = "";
            lblStatus1.Visible = false;
            lblStatus1.Refresh();
            lblElec.Visible = false;
            lblElec.Refresh();
            lblElection.Text = "";
            lblElection.Visible = false;
            lblElection.Refresh();
        }

        private bool CheckForHeaderFile(string filepath)
        {
            //check for file with extension .hdr.
            return filepath != null && Directory.EnumerateFiles(filepath, "*.hdr").Any();
        }

        //Method to get the file name of a specific file by providing its path and pattern(*.hdr) . returns only the first match found.
        private string GetFileName(string filePath, string searchPattern)
        {
            string fileName = null;
            if (filePath != null)
            {
                string[] fileEntries = Directory.GetFiles(filePath, searchPattern);
                int length = fileEntries.Length;
                if (length <= 0)
                {
                    //No file matching the searchPattern found.
                    MessageBox.Show(@"No file with extension " + searchPattern + @" found!");
                    EmptyLables();
                }
                else if (length > 1)
                {
                    //if there are more than one file with searchPattern, show the message to the user.
                    MessageBox.Show(@"Folder Contains more than 1 " + searchPattern + @" file. Cannot Proceed!");
                    EmptyLables();
                }
                else
                {
                    //found only one file with the searchPattern, find its file name.
                    string filteredFile = fileEntries[0];
                    fileName = Path.GetFileName(filteredFile);
                }
            }
            //return the first match found
            return fileName;
        }

        //Method to Get the file path of a chosen file pattern (Eg; *.hdr)from a chosen folder. Return only the first match found
        private string GetFilePath(string folderPath, string searchPattern)
        {
            string filePath = "";
            if (folderPath != null)
            {
                string[] fileEntries = Directory.GetFiles(folderPath, searchPattern);
                int length = fileEntries.Length;
                if (length <= 0)
                {
                    MessageBox.Show(@"No file with extension " + searchPattern + @" found!");
                }
                else if (length > 1)
                {
                    MessageBox.Show(
                        @"Folder Contains more than 1 " + searchPattern +
                        @" file. Cannot Process Return Single File Path Request!");
                }
                else
                {
                    //get only the first match
                    filePath = fileEntries[0];
                }
            }
            //retrun the file path of the file that matches the extension, searchPattern and folder as in the folderPath
            return filePath;
        }

        //User defined Method to clear textbox
        private void ClearTextBox(TextBox txtBox)
        {
            //clear the textbox send
            txtBox.Clear();
        }

        //User defined Method to clear validate TextBox for some value
        private bool IsTextValid(string value)
        {
            //if a valide string is passed
            if (value != null)
            {
                //if string is not empty and length of the string is NOT 0, then return true else return false.
                if (!String.IsNullOrEmpty(value) && value.Trim().Length != 0)
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        //Read Header file to get the precinct id and from there precinct
        private string SearchforHeaderFile(string headerPath)
        {
            string precinctName = null;
            if (headerPath != null)
            {
                //find the file name, complete path of the header file
                headerFilePath = GetFilePath(headerPath, searchPattern: "*.hdr");
                if (String.IsNullOrEmpty(headerFilePath))
                {
                    MessageBox.Show(@"No header file in the folder. Program exit");
                }
                else
                {
                    // Read the file and display it line by line.
                    try
                    {
                        //get Precinct Name passing the header path
                        precinctName = GetPrecinctName(headerFilePath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(@"Could not read the file");
                    }
                }
            }
            return precinctName;
        }

        private string GetPrecinctName(string path)
        {
            string location = "";
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    //once file is open, read the file line by line
                    var counter = 0;
                    string line;
                    bool flag = false; //flag for the word 'ELECTION'
                    while ((line = sr.ReadLine()) != null)
                    {
                        Console.WriteLine(line);
                        if (line.Trim() == "ELECTION")
                        {
                            //if found the word 'ELECTION' in the file, flag turns true.
                            flag = true;
                        }
                        if (flag)
                        {
                            //once line "ELECTION" is found start counter.
                            counter++;
                            if (counter == 3)
                            {
                                election = line.Trim();
                                addToElectionTable(election);
                                CheckDBExists(election);
                            }

                           

                            if (counter == 5)
                            {
                                //string tempFolderName = line.Trim();
                                //get equivalent precinct name to this precindtId from the database
                                try
                                {
                                    location = line.Trim();
                                    //precinct = GetPrecinctMatchFromDB(precinctId);
                                    break;
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(@"Could not read the file"); 
                                }
                            }
                        }
                    }
                    sr.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Could not read the file");
            }
            //return precinct name of the corresponding precinct ID
            return location;
        }

        //Get Precint Match from table 'PrecinctList'
        //private string GetPrecinctMatchFromDB(string precinctID)
        //{
        //    if (precinctID == null) throw new ArgumentNullException("precinctID");
        //    string queryResult = null;
        //    try
        //    {
        //        String tableName = "dbo.PrecinctList";
        //        select Precint based on the PrecinctCode from table PrecinctList.
        //        var query = String.Format("select Precinct from {0} where PrecinctCode='" + precinctID.ToUpper() + "'",
        //            tableName);
        //        using (SqlConnection connection = new SqlConnection(connectionString))
        //        {
        //             Create the Command and Parameter objects.
        //            SqlCommand command = new SqlCommand(query, connection);
        //            try
        //            {
        //                connection.Open();
        //                SqlDataReader rdr = command.ExecuteReader();
        //                while (rdr.Read())
        //                {
        //                    queryResult = rdr[0] as string;
        //                }
        //                rdr.Close();
        //            }
        //            catch (Exception ex)
        //            {
        //                MessageBox.Show(ex.Message);
        //            }
        //            Console.ReadLine();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(@"Could not read the file");
        //    }
        //    if (queryResult == null)
        //    {
        //        try
        //        {
        //            String tableName = "dbo.PrecinctList";
        //            select Precint based on the Precinct from table PrecinctList.
        //            var query = String.Format("select Precinct from {0} where Precinct='" + precinctID.ToUpper() + "'",
        //                tableName);
        //            using (SqlConnection connection = new SqlConnection(connectionString))
        //            {
        //                 Create the Command and Parameter objects.
        //                SqlCommand command = new SqlCommand(query, connection);

        //                 Open the connection in a try/catch block.
        //                 Create and execute the DataReader, writing the result
        //                try
        //                {
        //                    connection.Open();
        //                    SqlDataReader rdr = command.ExecuteReader();
        //                    while (rdr.Read())
        //                    {
        //                        queryResult = rdr[0] as string;
        //                    }
        //                    rdr.Close();
        //                    if (queryResult == null)
        //                    {
        //                        MessageBox.Show(
        //                            @"Precinct Does not exist in the List." + Environment.NewLine + @"Please  Cross Check with Precinct List in DB");
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    MessageBox.Show(ex.Message);
        //                }
        //                Console.ReadLine();
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show(@"Could not read the file");
        //        }
        //    }
        //    return queryResult;
        //}

        //Method called to update flag startCopy-endCopy.
        private bool UpdateCopiedPrecinctTable(string headerpath, string precinct, string copyStatus, string SerialNumberElectionLog)
        {
            bool status = false;
           
            //Insert into CopiedPrecinctTable when copyStatus=0
            if (copyStatus == "0")
            {
                try
                {
                    using (StreamReader sr = new StreamReader(headerpath))
                    {
                        //once file is open, read the file line by line
                        var counter = 0;
                        string line;//line for reading lines from the Electionlog.txt
                        string election = "";
                        
                        // election = "";
                        string dateTime = "";
                        int value = 0;
                        bool flag = false; //flag for the word 'ELECTION'
                        bool inserted = false;
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line.Trim() == "ELECTION")
                            {
                                //if found the word 'ELECTION' in the file, flag turns true.
                                flag = true;
                            }
                            if (flag)
                            {
                                //once line "ELECTION" is found start counter.
                                //set counter to 1 indicating-- line "ELECTION" as line1
                                counter++;
                                if (counter == 3)
                                {
                                    //get election name 3rd-in header file
                                    try
                                    {
                                        election = line.Trim();
                                       
                                        electionDbName = election;
                                        _message = election;

                                        electionDbName = Regex.Replace(electionDbName, "[,]", "_");
                                        electionDbName = Regex.Replace(electionDbName, "[' ']", "");
                                        
                                        fontColor = Color.Black;
                                        lblElec.Visible = true;
                                        lblElec.Refresh();
                                        lblElection.Visible = true;
                                        UpdateLabelStatus(lblElection, _message, fontColor);
                                        value++;//value=1. indicate election value is assigned.
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(@"Could not read the file");
                                    }
                                }
                                if (counter == 4)
                                {
                                    //get precinctId - 5th in header file
                                    try
                                    {
                                        precinctId = line.Trim();
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show("Exception" + ex);
                                    }
                                }
                                if (counter == 6)
                                {
                                    //get election date- 6th in header file
                                    try
                                    {
                                        dateTime = line.Trim();
                                        value++;//indicate both election and dataTime assigned.
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(@"Could not read the file");
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show(@"File does not contain the line 'ELECTION'." + Environment.NewLine +
                                                @"Please check the header file content." + Environment.NewLine + "" +
                                                @"System Exiting for safety reasons.");
                                // May be reading wrong file. Close everything down.
                                System.Windows.Forms.Application.Exit();
                            }
                            if (value == 2)
                            {
                                //insert values from election and dateTime into CopiedPresict folder
                                //try connecting  to precinctlist database and find a match for the tempFolderName
                                try
                                {
                                    String tableName = "[dbo.Memory_Log].[dbo].[CopiedLocations_" + electionDbName + "]";
                                    var query =
                                        String.Format("INSERT INTO " + tableName +
                                                      "(Election,Location,LocationIdCode,CopyFlag,CopyTime,SerialNumber) VALUES('" + election +
                                                      "','" + precinct + "','" + precinctId + "','" + copyStatus + "','" + DateTime.Now + "','" + SerialNumberElectionLog +
                                                      "')");
//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\EDIT//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\
                                    //Only add serial numbers that dont exists??? with the conformation to add or overwrite records that already exixts in the Database
                                    var records =String.Format("SELECT COUNT(*) FROM " + tableName + " WHERE Election= '" +
                                                     election + "' AND Location='" + precinct + "' AND SerialNumber = '" + SerialNumberElectionLog + "'");
                                    using (SqlConnection connection = new SqlConnection(connectionString))
                                    {
                                        SqlCommand record = new SqlCommand(records, connection);
                                        SqlCommand command = new SqlCommand(query, connection);
                                        try
                                        {
                                            int q = 0;
                                            connection.Open();
                                            SqlDataReader recordReader = record.ExecuteReader();
                                            // reads the first and only column count(*) and convert it to a number
                                            if (recordReader != null && recordReader.Read())
                                            {
                                                //Checking for Duplicate Entry;
                                                q = int.Parse(recordReader[0].ToString());
                                                recordReader.Close();
                                                connection.Close();
                                            }
                                            if (q == 0)
                                            {
                                                connection.Open();
                                                SqlDataReader rdr = command.ExecuteReader();
                                                status = true;
                                                if (rdr != null) rdr.Close();
                                                inserted = true;
                                            }
                                            else
                                            {
                                                // an entry already exist
                                                if (MessageBox.Show(@"Precinct " + precinct + @" for the Election " + Environment.NewLine + election + @" already exists in the DB. " + Environment.NewLine + @" DELETE Any Partially Copied Files?", @"Attention", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                                {
                                                    //YES 
                                                    bool cleaningStatus = UnWindUpdateTable(election);
                                                    if (!cleaningStatus)
                                                    {
                                                        MessageBox.Show(@"No Partially Added Records Found." + Environment.NewLine + precinct + @" for " + election + @" exists in the DB.");
                                                        //_message = @"No Partially Added Records Found." +
                                                        //           Environment.NewLine + precinct + @" for " + election +
                                                        //           @" exists in the DB.";
                                                        //UpdateLabelStatus(lblStatus1,_message, Color.Blue);
                                                        FillList(election, tableName); // Correct Tablename
                                                    }
                                                }
                                                //   ClearTextBox(txtSource);
                                                EmptyLables();
                                                break;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine(ex.Message);
                                            break;
                                        }
                                        Console.ReadLine();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(@"Could not read the file");
                                    break;
                                }
                                if (inserted)
                                {
                                    break;
                                }
                            }
                        }
                        sr.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(@"Could not read the file");
                }
                return status;
            }
            else if (copyStatus == "1")//copyStatus is 1 after successful copy of all 3 files(.hdr,electionlog, headerlog).
            {
                status = false;
                //try connecting  to precinctlist database and find a match for the tempFolderName
                try
                {
                    String tableName = "dbo.CopiedLocations_" + electionDbName;
                    var query =
                         String.Format("UPDATE " + tableName +
                                       " SET CopyFlag='" + copyStatus + "',CopyTime= '" + DateTime.Now +
                                       "' WHERE Election='" +
                                       election + "' AND Location ='" + precinct + "' AND SerialNumber = '"+SerialNumberElectionLog+"'");
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        SqlCommand command = new SqlCommand(query, connection);
                        try
                        {
                            connection.Open();
                            SqlDataReader rdr = command.ExecuteReader();
                            if (rdr != null) rdr.Close();
                            status = true;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(@"Could not read the file");
                    status = false;
                }
            }
            return status;
        }

        private void addToElectionTable(string election)
        {
            var Row_Count = 0;
            var Query = String.Format("IF OBJECT_ID('dbo.Election', 'U') IS NULL BEGIN " +
                                      "PRINT 'Creating table dbo.Election'" +
                                      "CREATE TABLE dbo.Election ([ID] [int] IDENTITY NOT NULL,[Election] [nvarchar] (max) NULL," +
                                      "CONSTRAINT [PK_dbo.Election] PRIMARY KEY CLUSTERED " +
                                      "([ID] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON))END");

                                      //"END ELSE SELECT COUNT(*) [Count] FROM dbo.Election WHERE Election = '"+ election+"'");
            var selecQuery = String.Format("SELECT COUNT(*) [Count] FROM dbo.Election WHERE Election = '" + election + "'");
            var insertQuery = String.Format("INSERT INTO dbo.Election (Election) Values('"+election+"')");
            using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Create the Command and Parameter objects.
                    SqlCommand command = new SqlCommand(Query, connection);
                    try
                    {
                        connection.Open();
                        SqlDataReader rowCountReader = command.ExecuteReader();
                        rowCountReader.Close();

                        command = new SqlCommand(selecQuery, connection);
                        rowCountReader = command.ExecuteReader();
                        rowCountReader.Read();
                        Row_Count = (int)rowCountReader["Count"];
                        rowCountReader.Close();
                                                
                        if(Row_Count <= 0)
                        {
                           command = new SqlCommand(insertQuery, connection );
                          
                           rowCountReader = command.ExecuteReader();                     

                        } 
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    FillCombo();
            }
        }
        private bool CheckDBExists(string ElectionName)
        {
            string[] tablesNames = new[] { "HeaderLog", "SystemLog", "ElectionLog", "CopiedLocations" };
            string[] TableNameForCreation = new[] { "", "", "", "" };
            var rowCount = 0;
            
            bool created = false;
           
            //var tables = "";
                
              for(int i=0; i< tablesNames.Length;i++)
              {
                TableNameForCreation[i] = tablesNames[i] + "_" + ElectionName;

                TableNameForCreation[i] = Regex.Replace(TableNameForCreation[i], "[,]", "_");
                TableNameForCreation[i] = Regex.Replace(TableNameForCreation[i], "[' ']", "");

             //   TableName[i].Replace(@", ", "_").Replace(@"' '", "_");

                var CheckTableExists = String.Format("Select COUNT(*) as [Count] FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = '" + TableNameForCreation[i] +"'");
            
                //String tableName = "dbo.PrecinctList";
                //select Precint based on the PrecinctCode from table PrecinctList.
                
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Create the Command and Parameter objects.
                    SqlCommand command = new SqlCommand(CheckTableExists, connection);
                    try
                    {
                        connection.Open();
                        SqlDataReader rowCountReader = command.ExecuteReader();
                        rowCountReader.Read();
                        rowCount = (int)rowCountReader["Count"];
                        if(rowCount <= 0)
                        {
                            created = createTables(TableNameForCreation[i], i);
                        }

                        rowCountReader.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    Console.ReadLine();
                }
              }           
                        
                // bool created = createTables(tablesNames);
                return created;            
        }

        private bool createTables(string tablenames, int j)
        {
            string createQuery = "";
           createQuery = getSqlStatement(tablenames,j);
            try
            {
                 using (SqlConnection connection = new SqlConnection(connectionString))
                {                     
                        SqlCommand cmd = new SqlCommand(createQuery, connection);
                        //cmd.Parameters.AddWithValue("@HeaderLog", tablenames[0]);
                        //cmd.Parameters.AddWithValue("@PK_Header_log_ElectionName", "PK_" + tablenames[0]);
                        connection.Open();
                        cmd.ExecuteNonQuery();                    
                    Console.WriteLine("Table Created Successfully...");
                    connection.Close();
                     return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("exception occured while creating table:" + e.Message + "\t" + e.GetType());
                return false;
            }
        }
        private string getSqlStatement(string tablenames, int tableNum)
        {
            // tablesNames = new[] { "HeaderLog", "SystemLog", "ElectionLog", "CopiedLocations" };
            string getSqlStatement= "";
            switch (tableNum) 
            {
                //Create HeaderLog Table with the election name 
                case 0:
                    getSqlStatement =  string.Format("CREATE TABLE [dbo].["+ tablenames+"] (" +
                "[ID] [int] IDENTITY NOT NULL,[Line1] [nvarchar](max) NULL,[Line2] [nvarchar](max) NULL," +
                "[Election] [nvarchar](max) NULL,[LocationIdCode] [nvarchar](max) NULL,[Location] [nvarchar](max) NULL,[serialNo] [nvarchar](max) NULL,[DateInfo] [nvarchar](max) NULL," +
                "[Line7] [nvarchar](max) NULL,[Line8] [nvarchar](max) NULL,[Line9] [nvarchar](max) NULL, [SerialNumber] [nvarchar](max) NULL," +
                " CONSTRAINT [PK_"+tablenames+"] PRIMARY KEY CLUSTERED " +
                "([ID] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]" +
                ") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]");
                    break;
                //Create SystemLog Table with the election name 
                case 1:
                    getSqlStatement = string.Format("CREATE TABLE [dbo].[" + tablenames + "](" +
                "[RNO] [int] IDENTITY NOT NULL,[Election] [nvarchar](max) NULL,[LocationIDCode] [nvarchar](max) NULL," +
                "[Location] [nvarchar](max) NULL,[LogNo] [nvarchar](max) NULL,[Code] [nvarchar](max) NULL,[LogDate] [datetime] NULL," +
                "[LogTime] [nvarchar](max) NULL,[LogSet] [nvarchar](max) NULL,[SerialNo] [nvarchar](max) NULL," +
                "[LogType] [nvarchar](max) NULL,[Log] [nvarchar](max) NULL,[TrimedLog] [nvarchar](max) NULL," +
                "[SerialNumber] [nvarchar](max) NULL, CONSTRAINT [PK_" + tablenames + "] PRIMARY KEY CLUSTERED " +
                "([RNO] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]" +
                ") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]");
                    break;
                //Create ElectionLog Table with the election name 
                case 2:
                    getSqlStatement = string.Format("CREATE TABLE  [dbo].[" + tablenames + "](" +
                "[ID] [int] IDENTITY NOT NULL,[Location] [nvarchar](max) NULL,[LocationIDCode] [nvarchar](max) NULL," +
                "[Election] [nvarchar](max) NULL,[SNumber] [nvarchar](max) NULL,[Code] [nvarchar](max) NULL,[LogDate] [datetime] NULL," +
                "[LogTime] [nvarchar](max) NULL,[LogSet] [nvarchar](max) NULL,[SerialNo] [nvarchar](max) NULL," +
                "[LogType] [nvarchar](max) NULL,[Log] [nvarchar](max) NULL,[TrimedLog] [nvarchar](max) NULL," +
                "[SerialNumber] [nvarchar](max) NULL, CONSTRAINT [PK_" + tablenames + "] PRIMARY KEY CLUSTERED " +
                "([ID] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]" +
                ") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]");
                    break;
                //Create CopiedLocations Table with the election name 
                case 3:
                    getSqlStatement = string.Format("CREATE TABLE [dbo].[" + tablenames + "](" +
                "[RNO] [int] IDENTITY NOT NULL,[Election] [nvarchar](max) NULL,[Location] [nvarchar](max) NULL," +
                "[LocationIDCode] [nvarchar](max) NULL,[CopyFlag] [nvarchar](max) NULL,[CopyTime][nvarchar](max) NULL,[SerialNumber] [nvarchar](max) NULL" +
                ", CONSTRAINT [PK_" + tablenames + "] PRIMARY KEY CLUSTERED " +
                "([RNO] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]" +
                ") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]");
                    break;
            }
            return getSqlStatement;           
        }

        private bool InsertHeaderFile(string headerPath)
        {
            bool headerTableInsert = false;
            bool DBSexists = false;
            string[] lines = File.ReadAllLines(headerPath);
            string[] value = new string[9];
            bool flag = false; //flag for the string 'ELECTION'
            //once file is open, read the file line by line
            var counter = 0;
            int i = 0;
            foreach (string line in lines)
            {
                if (line.Trim() == "ELECTION")
                {
                    counter++;
                    flag = true;
                }
                if (flag && (counter == 1))
                {
                    if (!string.IsNullOrEmpty(line.Trim()))
                        value[i] = line.Trim();
                    i++;
                }
            }

           //  DBSexists = CheckDBExists(value[2]);

            //try connecting  to precinctlist database and find a match for the tempFolderName            
             
                 try
                 {
                     String tableName = "dbo.HeaderLog_" + electionDbName;
                     if (value != null)
                     {
                         election = value[2];
                         precinctIdCode = value[3];
                         precinctId = value[4];
                         var query1 =
                             String.Format("INSERT INTO " + tableName +
                                            "(Line1,Line2,Election,LocationIDCode,Location,DateInfo,Line7,Line8,Line9, SerialNumber) VALUES('" +
                                            value[0] + "','" + value[1] + "','" + value[2] + "','" + value[3] + "','" +
                                            value[4] + "','" + value[5] + "','" + value[6] + "','" + value[7] + "','" +
                                            value[8] + "'"+",'"+ SerialNumberElectionLog+"')");
                         using (SqlConnection connection = new SqlConnection(connectionString))
                         {
                             SqlCommand command = new SqlCommand(query1, connection);
                             try
                             {
                                 connection.Open();
                                 SqlDataReader rdr = command.ExecuteReader();
                                 // startToCopy = true;
                                 if (rdr != null) rdr.Close();
                                 headerTableInsert = true;
                             }
                             catch (Exception ex)
                             {
                                 MessageBox.Show(ex.Message);
                             }
                             Console.ReadLine();
                         }
                     }
                 }
                 catch (Exception ex)
                 {
                     MessageBox.Show(@"Could not read the file");
                 }
             
            return headerTableInsert;
        }


        private FileInfo[] SearchforSerialNumber(string path)
        {
            // Get the files
            DirectoryInfo info = new DirectoryInfo(path);
            FileInfo[] files = info.GetFiles("encrypted.ms.public.*", SearchOption.TopDirectoryOnly);
            return files;

        }//end SearchforSerialNumber()

        private bool InsertSerialNumber(string ele, string preci)
        {
            bool serialNumberInsert = false;
            for (int scount = 0; scount < serialnos.Length; scount++)
            {
                //   DateTime time1 = DateTime.Parse(serialnos[scount].LastWriteTime.ToString("MM/dd/yyyy HH:mm:ss"));

                DateTime time1 = DateTime.Parse(serialnos[scount].CreationTime.ToString("yyyy-MM-dd hh:mm:ss"));
                string Time1 = time1.ToString("yyyy-MM-dd hh:mm:ss ");
                time1 = Convert.ToDateTime(Time1);
                
                //string Sdate = sDate.ToString("yyyy-MM-dd hh:mm:ss ");

                //highest date and time in the array
                serialNo = serialnos[scount].ToString().Split('.').Last();
                //try connecting  to precinctlist database and find a match for the tempFolderName
                try
                {
                    string tableName = "dbo.ElectionLog_"+electionDbName;
                    //query for updating the ElectionLog with the respective serialnumber
                    var query =
                        String.Format("UPDATE " + tableName +
                                      " SET SerialNo=" + serialNo + " WHERE LogDate >= '" + time1 + "' AND Location ='" + preci + "' AND Election='" + ele + "'");
                    //query for updating the SystemLog with the respective serialnumber
                    tableName = "dbo.SystemLog_" + electionDbName;
                    var query1 =  
                        String.Format("UPDATE " + tableName + " SET SerialNumber = " + serialNo + " WHERE LogDate >=  '" + time1 + "' AND Location ='" + preci + "' AND Election='" + ele + "'");
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        using (SqlConnection connection1 = new SqlConnection(connectionString))
                        {
                            //command to update ElectionLog
                            SqlCommand command = new SqlCommand(query, connection);
                            try
                            {
                                connection.Open();

                                //command to update SystemLog
                                SqlCommand command1 = new SqlCommand(query1, connection1);
                                try
                                {                                    
                                    connection1.Open();
                                    //reader for ElectionLog
                                    SqlDataReader rdr = command.ExecuteReader();
                                    //reader for SystemLog
                                    SqlDataReader rdr1 = command1.ExecuteReader();
                                    if (rdr != null && rdr1 != null)
                                    {
                                        //close both Election reader and System reader
                                        rdr.Close();
                                        rdr1.Close();
                                        //set serialNumberInsert to true. This means SerialNumber update successful.
                                        serialNumberInsert = true;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(@"Could not read the file");
                }

            } //end find right serialnumber
            return serialNumberInsert;
        } //end InsertSerialNumber(a,b)


        /// <summary>
        /// User defined Method to Copy ElectionLog to DB
        /// </summary>
        /// <param name="electionPath">the user selected source path.</param>
        /// <returns>returns if true if insert to ElectionLog is succesful.</returns>
        private bool InsertElectionFile(string electionPath)
        {
            //Election file is in 'log' folder inside _selectedSourcePath
            bool electionTableInsert = false;
            string searchPath = electionPath + "\\log";
            string headerName = GetFilePath(searchPath, searchPattern: "Election.log");
            String tableName = "dbo.ElectionLog_"+electionDbName;
            string[] lines = File.ReadAllLines(headerName);//Opens a text file, reads all lines of the file, and then closes the file.
            string[] value = new string[10]; //assumption: there will be only MAX 10 columns to read.
            string tLog = null;//trimmeLog.
            bool flag = false; //flag for the word 'ELECTION'.

            //once file is open, read the file line by line
            var counter = 0;
            int i = 0;
            serialNo = "";
            string getSno = null;
            //Establish database connection using connection string
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                //open db connnection
                connection.Open();

                //assign default value for the serial number as 0
                serialNo = "0";
                bool flagserial = false;
                int snocounter = 0;

                //read each line from the electionlog file 
                foreach (string line in lines)
                {
                    value = line.Split(',');

                    //combine date and time to LogDate field (LodDate column will contain date and time).
                    value[1] = value[1] + value[2];

                    //try connecting  to precinctlist database and find a match for the tempFolderName
                    try
                    {
                        if (value != null)
                        {
                            int cntValue = 0;//used for log concatenation.
                            int cntindex = 0;//used for log concatenation.
                            foreach (string word in value)
                            {
                                cntindex++;

                                //search if the line contains either of the three strings, if found replace original value with the matched string 
                                const string searchString1 = "Keys detected on poll media";
                                const string searchString2 = "IMR Log Characteristic Point Status";
                                const string searchString3 = " IMR Log Characteristic Point Status";//white space in the beginning.
                                if (word.Trim().Contains(searchString1))
                                {
                                    tLog = searchString1;
                                }
                                else if ((word.Trim().Contains(searchString2)) ||
                                         (word.Trim().Contains(searchString3)) ||
                                         ((word.TrimStart().TrimEnd().Contains(searchString3)) ||
                                          ((word.Contains(searchString2)))))
                                {
                                    tLog = searchString2;
                                }
                                else if (!(cntValue >= 1) && (cntindex < 7))
                                {
                                    //if not any among the searchStrings, value[6] is assigned to tLog.
                                    tLog = value[6];
                                }

                                //all string after 5th ',' is taken under 'log' column
                                //concatenate  all string from value[7] onwards with value[6] 
                                if (value.Length > 7 && word == value[7])
                                {
                                    cntValue++;
                                }
                                if (cntValue >= 1)
                                {
                                    value[6] += "," + word;
                                }
                            }//end foreach value

                            var query =
                                String.Format("INSERT INTO " + tableName +
                                              "(Location,LocationIdCode,Election,SerialNumber,Code,LogDate,LogTime,LogSet,SerialNo,LogType,Log,TrimedLog) VALUES('" +
                                              precinct.Trim() + "','" + 
                                              precinctIdCode.Trim() + "','" +
                                              election.Trim() + "','" + 
                                              SerialNumberElectionLog + "','" + 
                                              value[0].Trim() + "','" +
                                              value[1].Trim() + "','" +
                                              value[2].Trim() + "','" + 
                                              value[3].Trim() + "','" +
                                              serialNo        + "','" + 
                                              value[5].Trim() + "','" +
                                              value[6].Trim() + "','" +
                                              tLog.Trim() + "')");
                            SqlCommand command = new SqlCommand(query, connection);
                            try
                            {
                                SqlDataReader rdr = command.ExecuteReader();
                                rdr.Close();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                                electionTableInsert = false;
                            }

                        }//end if value exists
                        electionTableInsert = true;
                    }//end try portion of try-catch
                    catch (Exception ex)
                    {
                        MessageBox.Show(@"Could not read the file");
                        electionTableInsert = false;
                        break;
                    }//end catch
                }//end for each line
            }//end connection
            return electionTableInsert;
        }//end InsertElectionfile 

        //User defined Method to Copy SystemLog to DB
        private bool InsertSystemFile(string systemPath)
        {
            //Election file is in 'log' folder inside _selectedSourcePath
            bool systemTableInsert = false;
            string searchPath = systemPath + "\\log";
            string headerName = GetFilePath(searchPath, searchPattern: "System.log");
            String tableName = "dbo.SystemLog_"+electionDbName;
            string[] lines = File.ReadAllLines(headerName);//Opens a text file, reads all lines of the file, and then closes the file.
            string[] value = new string[7];//assumption: no more than 7 values(columns) to read from the headerlog text.
            bool flag = false; //flag for the word 'ELECTION'
            //fill column SerialNumber with 0 initially.
            serialNo = "0";
            //once file is open, read the file line by line
            var counter = 0;
            int i = 0;
            string getSno = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command1 = null;
                foreach (string line in lines)
                {
                    if (line != null) value = line.Trim().Split(',');
                    try
                    {
                        if (value != null)
                        {
                            int cntValue = 0;
                            //combine date and time to LogDate field (LodDate column will contain date and time).
                            value[1] = value[1] + value[2];
                            //Merge long comma seperated log to column Log
                            foreach (string word in value)
                            {
                                if (value.Length > 7 && word == value[7])
                                {
                                    cntValue++;
                                }
                                if (cntValue >= 1)
                                {
                                    value[6] += "," + word;
                                }

                            }//end search by word
                            //try connecting  to precinctlist database and find a match for the tempFolderName
                            try
                            {
                                if (value != null)
                                {//Column 'SerialNumber' is the actual machine serial number
                                    var query =
                                        String.Format("INSERT INTO " + tableName +
                                                      "(Location,LocationIDCode, Election,Code,LogDate,LogTime,LogSet,LogNo,LogType,Log,SerialNo,SerialNumber) VALUES('" +
                                                      precinct.Trim() + "','" + 
                                                      precinctIdCode.Trim() + "','" +
                                                      election.Trim() + "','" + 
                                                      value[0].Trim() + "','" +
                                                      value[1].Trim() + "','" +
                                                      value[2].Trim() + "','" + 
                                                      value[3].Trim() + "','" +
                                                      value[4].Trim() + "','" +
                                                      value[5].Trim() + "','" +
                                                      value[6].Trim() + "','" +
                                                      serialNo        + "','" +
                                                      SerialNumberElectionLog + "')");
                                    SqlCommand command = new SqlCommand(query, connection);
                                    try
                                    {
                                        SqlDataReader rdr = command.ExecuteReader();
                                        rdr.Close();

                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(ex.Message);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(@"Could not read the file");
                                systemTableInsert = false;
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(@"MESSAGE: " + ex);
                    }
                }
            }
            systemTableInsert = true;
            return systemTableInsert;
        }//end InsertSystemFile()

        private void UpdateLabelStatus(Label label, string message, Color fontColor)
        {
            label.Visible = true;
            label.ForeColor = fontColor;
            label.Text = _message;
            label.Refresh();
        }

        private void FillList(string election, string tableName)
        {
            Label label = lblElection;
            string eletionDbName = "";
            lblElec.Visible = true;
            _message = election;
            fontColor = Color.Black;
            UpdateLabelStatus(label, _message, fontColor);
            lstPrecinct.ResetText();
            lstPrecinct.Refresh();
            lstPrecinct.Items.Clear();
            string query;
            string queryCount;
            int count = 0;
            if (election == null)
            {
                election = "";
            }
            eletionDbName = election;

            eletionDbName = Regex.Replace(eletionDbName, "[,]", "_");
            eletionDbName = Regex.Replace(eletionDbName, "[' ']", "");
              
            query = String.Format("SELECT DISTINCT Location FROM " + tableName + " WHERE Election='" +
                                          election + "' AND CopyFlag='1' order by Location DESC");           

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    try
                    {
                        connection.Open();
                        SqlDataReader recordReader = command.ExecuteReader();
                        lstPrecinct.Items.Clear();
                        while (recordReader != null && recordReader.Read())
                        {
                             
                            string electionList = recordReader["Location"].ToString();
                            using (SqlConnection connection1 = new SqlConnection(connectionString))
                            {
                                queryCount = String.Format("Select COUNT(*) AS [Counter] From " + tableName + " Where Location = '"+ electionList +"' AND Copyflag = '1'");
                                SqlCommand command1 = new SqlCommand(queryCount, connection1);                              
                                try
                                {

                                    connection1.Open();
                                    SqlDataReader recordCount = command1.ExecuteReader();
                                    recordCount.Read();
                                    var recoundCount =(int) recordCount["Counter"];
                                    electionList = recoundCount.ToString() +" "+electionList;
                                    count += (int)recordCount["Counter"];
                                    recordCount.Close();
                                    connection1.Close();
                                }
                                catch (Exception ex)
                                {
                                     MessageBox.Show(ex.Message + @". System Exiting");
                                    System.Windows.Forms.Application.Exit();
                                }  
                            }
                            lstPrecinct.Visible = true;                            
                            lstPrecinct.Items.Add(electionList);
                            lstPrecinct.Refresh();                            
                            lblCount.Text = count.ToString();
                            lblCopiedPrecints.Visible = true;
                            lblCopiedPrecints.ForeColor = Color.DarkRed;
                            lblCount.Visible = true;
                            lblCount.ForeColor = Color.DarkRed;
                            lblCount.Refresh();
                        }
                        if (recordReader != null) recordReader.Close();
                        connection.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + @". System Exiting");
                        System.Windows.Forms.Application.Exit();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Could not read the file");
                System.Windows.Forms.Application.Exit();
            }
            lstPrecinct.Refresh();
        }
        private void GenerateSummaryReport()
        {
            //GenerateSummaryReportComplied();
            // Start a new workbook in Excel.
            m_objExcel = new Application();
            m_objBooks = (Workbooks)m_objExcel.Workbooks;
            m_objBook = (_Workbook)(m_objBooks.Add(m_objOpt));
            m_objSheets = (Sheets)m_objBook.Worksheets;
            m_strSampleFolder = repDestn;
            List<String> precinctList = new List<String>();
            

            // Create an array for the headers
            object[] objHeaders = null;
            try
            {
                string table = null;
                string electionChoice = repElection;
                string electionDbName = electionChoice;
                string SerialNumber = "";
                DateTime sDate = repSdate;
                DateTime eDate = repEdate;
                var errorQuery = "";
                var query = "";
                SqlDataReader rdr = null;
                SqlCommand command = null;
                var queryReadLog = "";
                bool filterFlag = false;

                electionDbName = Regex.Replace(electionDbName, "[,]", "_");
                electionDbName = Regex.Replace(electionDbName, "[' ']", "");


                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String QueryLocation = String.Format("SELECT DISTINCT Location"
                    + " AS Location FROM dbo.ElectionLog_"+electionDbName+" WHERE Election = @electionChoice ORDER BY Location ");

                    command = new SqlCommand(QueryLocation, connection);
                    //command.Parameters.AddWithValue("@ElectionLog", "dbo.ElectionLog_" + electionDbName);
                    command.Parameters.AddWithValue("@electionChoice", electionChoice);
                    rdr = command.ExecuteReader();
                    while (rdr.Read())
                    {
                        precinctList.Add(rdr[0].ToString());
                    }
                    rdr.Close();
                    // precinctList.Add("Last");

                    if (radioElection.Checked)
                    {
                        table = "dbo.ElectionLog_"+electionDbName;
                        objHeaders = new[]
                        {
                            "Election Title", "Location Name", "Location Id",
                            "Election Date","LogTime","Category",
                            "Event Code", "Description", "Serial Number"
                        };
                        if (electionChoice != null && chkFilter.Checked)
                        {
                            filterFlag = true;

                            // all required options selected.
                            //filter is selected.
                            queryReadLog =
                                String.Format(" SELECT  Election as ElectionTitle,Location as LocationName,LocationIdCode as LocationIdCode,LogDate as ElectionDate,LogTime as ElectionTime,"
                                + " LogType as Category,Code as EventCode,Log as Description,SerialNumber FROM "
                                + table + " WHERE Election ='" + electionChoice + "' AND Location LIKE  @precinct AND LogDate >=  @SD  AND LogDate<= @ED ORDER BY LogType,Location,LogDate,LogTime ");
                            //REMOVED LogTime. Get LogDate and LogTime using single column.
                            //queryReadLog =
                            //   String.Format("");

                            errorQuery = String.Format("SELECT count(LogType) as [ErrorCount] FROM " + table + " WHERE Election='" +
                                             electionChoice + "'AND Location LIKE  @precinct AND [LogType] = 'P_ERROR' AND LogDate >= @SD  AND LogDate <= @ED ");

                            //Get the RowCount.
                            query =
                                String.Format("SELECT count(*) as [RowCount] FROM " + table + " WHERE Election='" +
                                              electionChoice + "' AND Location LIKE  @precinct AND LogDate >= @SD  AND LogDate <= @ED");
                        }
                        else if (electionChoice != null && (chkFilter.Checked == false))
                        {
                            // all required options selected.
                            //filter not selected.
                            queryReadLog =
                                String.Format(" SELECT  Election as ElectionTitle,Location as LocationName,LocationIdCode as LocationIdCode,LogDate as ElectionDate,LogTime as ElectionTime,"
                                + " LogType as Category,Code as EventCode,Log as Description,SerialNumber FROM "
                                + table + " WHERE Election ='" + electionChoice + "' AND Location LIKE  @precinct ORDER BY LogType,Location,LogDate,LogTime ");
                            //REMOVED LogTime. Get LogDate and LogTime using single column.
                            errorQuery = String.Format("SELECT count(LogType) as [ErrorCount] FROM " + table + " WHERE Election='" +
                                             electionChoice + "'AND Location LIKE  @precinct AND [LogType] = 'P_ERROR'");
                            //Get the RowCount.
                            query = String.Format("SELECT count(*) as [RowCount] FROM " + table + " WHERE Election='" +
                                              electionChoice + "'AND Location LIKE  @precinct");

                        }
                    }
                    else if (radioSystem.Checked)
                    {
                        table = "dbo.SystemLog_" + electionDbName;
                        objHeaders = new[]
                        {
                          "Election Title", "Location Name", "Location Id","Election Date", "Election Time", "Category", "Event Code",
                          "Description", "Serial Number"
                        };
                        if (electionChoice != null && chkFilter.Checked)
                        {
                            filterFlag = true;
                            // all required options selected.
                            //filter is selected.
                            queryReadLog =
                                String.Format("SELECT Election as ElectionTitle,Location as LocationName,LocationIdCode,LogDate as ElectionDate,LogTime as ElectionTime, " +
                                    "LogType as Category,Code as EventCode,Log as Description,SerialNumber  FROM " +
                                    table + " WHERE Election='" + electionChoice + "' AND Location LIKE  @precinct" +
                                    " AND LogDate >= @SD  AND LogDate<=@ED ORDER BY LogType,Location,LogDate");

                            errorQuery = String.Format("SELECT count(LogType) as [ErrorCount] FROM " + table + " WHERE Election='" +
                                             electionChoice + "'AND Location LIKE  @precinct AND [LogType] = 'P_ERROR' AND LogDate >= @SD  AND LogDate<=@ED");

                            //Get the RowCount.
                            query =
                                String.Format("SELECT count(*) as [RowCount] FROM " + table + " WHERE Election='" +
                                              electionChoice + "' AND Location LIKE  @precinct AND LogDate >= @SD  AND LogDate<=@ED");
                        }
                        else if (electionChoice != null && (chkFilter.Checked == false))
                        {
                            // all required options selected.
                            //filter not selected.
                            queryReadLog =
                                String.Format("SELECT Election as ElectionTitle,Location as LocationName,LocationIdCode,LogDate as ElectionDate,LogTime as ElectionTime, " +
                                    "LogType as Category,Code as EventCode,Log as Description,SerialNumber FROM " +
                                    table + " WHERE Election='" + electionChoice + "' AND Location LIKE  @precinct" +
                                    " ORDER BY LogType,Location,LogDate,LogTime ");

                            errorQuery = String.Format("SELECT count(LogType) as [ErrorCount] FROM " + table + " WHERE Election='" +
                                             electionChoice + "'AND Location LIKE  @precinct AND [LogType] = 'P_ERROR'");

                            //Get the RowCount.
                            query =
                                String.Format("SELECT count(*) as [RowCount] FROM " + table + " WHERE Election='" +
                                              electionChoice + "'AND Location LIKE  @precinct");
                        }
                    }
                    else
                    {
                        MessageBox.Show(@"Cannot Proceed. Log Choice is Empty.");
                    }

                    int cnt = 0;
                    for (int l = 0; l < precinctList.Count(); l++)
                    {
                        String precinct = precinctList[l].ToString();                        
                        //command.Parameters.AddWithValue("@SD", Convert.ToDateTime(Sdate));


                        command = new SqlCommand(errorQuery, connection);

                        //string Sdate = sDate.ToString("yyyy-MM-dd hh:mm:ss tt");
                        //string Edate = eDate.ToString("yyyy-MM-dd hh:mm:ss tt");

                        string Sdate = sDate.ToString("yyyy-MM-dd hh:mm:ss ");
                        string Edate = eDate.ToString("yyyy-MM-dd hh:mm:ss ");



                        command.Parameters.AddWithValue("@precinct", precinct );
                        command.Parameters.AddWithValue("@SD", Convert.ToDateTime(Sdate));
                        command.Parameters.AddWithValue("@ED", Convert.ToDateTime(Edate));

                        SqlDataReader errorReader = command.ExecuteReader();
                        errorReader.Read();
                        var ErrorCount = (int)errorReader["ErrorCount"];
                        errorReader.Close();

                        command = new SqlCommand(query, connection);
                        // command.Parameters.AddWithValue("@precinct","'%" + precinct +"' -%'");
                        command.Parameters.AddWithValue("@precinct", precinct );
                        command.Parameters.AddWithValue("@SD", Convert.ToDateTime(Sdate));
                        command.Parameters.AddWithValue("@ED", Convert.ToDateTime(Edate));

                        //
                        SqlDataReader rowCountReader = command.ExecuteReader();
                        rowCountReader.Read();
                        var rowCount = (int)rowCountReader["RowCount"];
                        rowCountReader.Close();

                        command = new SqlCommand(queryReadLog, connection);
                        if (queryReadLog != String.Empty)
                        {
                            command.Parameters.AddWithValue("@SD", Convert.ToDateTime(Sdate));
                            command.Parameters.AddWithValue("@ED", Convert.ToDateTime(Edate));
                        }
                        command.Parameters.AddWithValue("@precinct", precinct );
                        rdr = command.ExecuteReader();
                        //Get the ColumnCount.
                        int columnCount = rdr.FieldCount;

                        Object[,] filedValues = new Object[rowCount, columnCount];
                        int i = 0;
                        int firstRecord = 0;
                        //removeCharacters(precinctList[l]);
                        string distinctPrecinct = precinctList[l];
                        string currentPrecinct = null;
                        Worksheet sheet;

                        int tillHere = 0;
                        bool nextsheet = false;
                        object False = false;
                        object True = true;
                        while (rdr.Read())
                        {
                            for (int j = 0; j < columnCount; j++)
                            {
                                filedValues[i, j] = Convert.ToString(rdr[j]);

                            }
                            i++;
                        }
                        rdr.Close();
                        //if (cnt == 0)
                        //{
                        cnt++;
                        m_objSheet = (Worksheet)m_objBook.Worksheets.Item[cnt];
                        // m_objSheet.Name = distinctPrecinct;
                        m_objSheet.Name = distinctPrecinct.Length > 29 ? distinctPrecinct.Remove(29) : distinctPrecinct;
                        m_objSheet.Select();
                        //}

                        if (i > 0)
                        {
                            m_objSheet = (Worksheet)m_objSheets.get_Item(cnt);
                            //m_objSheet.Name = distinctPrecinct;
                            m_objSheet.Name = distinctPrecinct.Length > 29 ? distinctPrecinct.Remove(29) : distinctPrecinct;
                            m_objSheet.Select();
                            m_objRange = m_objSheet.get_Range("A1", m_objOpt);
                            m_objRange = m_objRange.get_Resize(1, columnCount);
                            m_objRange.Value = objHeaders;
                            m_objFont = m_objRange.Font;
                            m_objFont.Bold = true;
                            //Add Table Style 
                            m_objRange.Worksheet.ListObjects.Add(XlListObjectSourceType.xlSrcRange,
                            m_objRange, System.Type.Missing, XlYesNoGuess.xlYes, System.Type.Missing).Name = "Compiled";
                            m_objRange.Select();
                            m_objRange.Worksheet.ListObjects["Compiled"].TableStyle = "TableStyleMedium3";
                            // Add array value to the worksheet starting at cell A2.
                            m_objRange = m_objSheet.get_Range("A2", m_objOpt);
                            m_objRange = m_objRange.Resize[i, columnCount];


                            m_objRange.Value = filedValues;

                            m_objRange.EntireColumn.AutoFit();


                            if (ErrorCount > 0)
                            {
                                //if column logtype is p_error change color to red
                                for (int row = 0; row < ErrorCount; row++)
                                {
                                    for (int c = 0; c < columnCount; c++)
                                    {
                                        m_objExcel.Cells[row + 2, c + 1].Interior.Color = ColorTranslator.ToOle(Color.Red);
                                    }

                                    // m_objExcel.Rows[row+1].Interior.Color = ColorTranslator.ToOle(Color.Red);

                                }
                            }
                            m_objRange.Sort(m_objRange.Columns[2, Type.Missing], XlSortOrder.xlAscending, m_objRange.Columns[4, Type.Missing], Type.Missing, XlSortOrder.xlAscending,
                                Type.Missing, XlSortOrder.xlAscending,
                                XlYesNoGuess.xlYes, Type.Missing, Type.Missing,
                                XlSortOrientation.xlSortColumns,
                                XlSortMethod.xlPinYin,
                                XlSortDataOption.xlSortNormal,
                                XlSortDataOption.xlSortNormal,
                                XlSortDataOption.xlSortNormal);


                            //search for the string and color the row in Red.
                            //foreach (Range usedRange in m_objSheet.UsedRange.Rows)
                            //{
                            //    //Apply Red color to rows containing Error code
                            //    Range findRng = usedRange.Find("P_ERROR", Missing.Value,
                            //        XlFindLookIn.xlValues,
                            //        Missing.Value, Missing.Value, XlSearchDirection.xlNext, False, False,
                            //        Missing.Value);
                            //    if (findRng != null)
                            //    {
                            //        usedRange.Interior.Color =
                            //        ColorTranslator.ToOle(Color.Red);
                            //    }
                            //}
                            i = 0;
                            // cnt++;
                            //if (cnt < 4)
                            //{
                            //    m_objSheet = (Worksheet)m_objSheets.get_Item(cnt);
                            //    //  m_objSheet.Name = distinctPrecinct;
                            //    m_objSheet.Name = distinctPrecinct.Length > 29 ? distinctPrecinct.Remove(29) : distinctPrecinct;
                            //    m_objSheet.Select();
                            //}
                            //else
                            //{
                            Int32 sheetcount = m_objBook.Worksheets.Count;
                            m_objSheet = (Worksheet)m_objBook.Worksheets.Add(Type.Missing, m_objBook.Worksheets[sheetcount], 1, XlSheetType.xlWorksheet);
                            //    //m_objSheet.Name = distinctPrecinct;
                            //    m_objSheet.Name = distinctPrecinct.Length > 29 ? distinctPrecinct.Remove(29) : distinctPrecinct;
                            //    m_objSheet.Select();
                            //}
                        }


                    }
                    string reportTime = DateTime.Now.ToString("HHmmss");
                    string reportName = "";

                    if (filterFlag)
                    {
                        reportName = "Filtered_Summary Report_" + "_" + table + "_" +
                                     reportTime + ".xlsx";
                    }
                    else
                    {
                        reportName = "Summary Report_" + "_" + table + "_" + reportTime +
                                     ".xlsx";
                    }


                    //string reportName = @"Summary Report_" + table + "_" + reportTime +
                     //                   ".xlsx";
                    string reportPath = string.Format("{0}{1}", m_strSampleFolder, reportName);
                    //save the workbook
                    m_objBook.SaveAs(reportPath, m_objOpt, m_objOpt,
                        m_objOpt, m_objOpt, m_objOpt, XlSaveAsAccessMode.xlNoChange,
                        m_objOpt, m_objOpt, m_objOpt, m_objOpt);                   
                    //Release excel objects
                    if (m_objExcel != null) //&& Marshal.ReleaseComObject(m_objExcel) != 0)
                    {
                        Marshal.ReleaseComObject(m_objRange);
                        Marshal.ReleaseComObject(m_objSheet);
                        Marshal.ReleaseComObject(m_objSheets);
                        Marshal.ReleaseComObject(m_objBook);
                        Marshal.ReleaseComObject(m_objBooks);
                        m_objExcel.Quit();
                        Marshal.ReleaseComObject(m_objExcel);
                    }
                    //close table data reader
                    rdr.Close();
                    connection.Close();
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    MessageBox.Show(@"Summary Report created at " + reportPath);

                    //If Summary Report create successfully, create compiled report. 

                    GenerateSummaryReportComplied();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex);
                m_objBook.Close(false, m_objOpt, m_objOpt);
                //Release excel objects
                if (m_objExcel != null && Marshal.ReleaseComObject(m_objExcel) != 0)
                {
                    Marshal.ReleaseComObject(m_objRange);
                    Marshal.ReleaseComObject(m_objSheet);
                    Marshal.ReleaseComObject(m_objSheets);
                    Marshal.ReleaseComObject(m_objBook);
                    Marshal.ReleaseComObject(m_objBooks);
                    m_objExcel.Quit();
                    Marshal.ReleaseComObject(m_objExcel);
                }
            }
        }

        private void GenerateSummaryReportComplied()
        {
            // Start a new workbook in Excel.
            m_objExcel = new Application();
            m_objBooks = (Workbooks)m_objExcel.Workbooks;
            m_objBook = (_Workbook)(m_objBooks.Add(m_objOpt));
            m_objSheets = (Sheets)m_objBook.Worksheets;
            m_objSheet = (_Worksheet)(m_objSheets.get_Item(1));
            m_strSampleFolder = repDestn;
            StringBuilder Logs = new StringBuilder();

           // string electionChoice = repElection;
            string electionDbName = repElection;
            
            electionDbName = Regex.Replace(electionDbName, "[,]", "_");
            electionDbName = Regex.Replace(electionDbName, "[' ']", "");


            // Create an array for the headers and add it to cells A1:C1.
            object[] objHeaders = null;
            try
            {
                string table = null;
                string electionChoice = repElection;
                string SerialNumber = "";
                bool filterFlag = false;
                string querryLogs = "";
                DateTime sDate = repSdate;
                DateTime eDate = repEdate;
                var query = "";
                SqlDataReader rdr = null;
                SqlCommand command = null;
                var queryReadLog = "";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    ////////////////////////////////////////////////////////////////////////EDIT////////////////////////////////////////////////////////////////////////
                    querryLogs = "SELECT DISTINCT QuoteName(TrimedLog) +',' as trimedLog  From dbo.ElectionLog_" + electionDbName + " Order BY TrimedLog";


                    command = new SqlCommand(querryLogs, connection);
                   // command.Parameters.AddWithValue("@ElectionLog", "dbo.ElectionLog_" + electionDbName);
                    SqlDataReader logReader = command.ExecuteReader();


                    while (logReader.Read())
                    {

                        Logs.Append(logReader[0].ToString());
                    }
                    Logs.Length--;
                    logReader.Close();

                    if (radioElection.Checked)
                    {
                        table = "dbo.ElectionLog_" + electionDbName;
                        if (electionChoice != null && chkFilter.Checked)
                        {
                            // all required options selected.
                            //filter is selected.
                            queryReadLog = String.Format(
                                                            "SELECT * FROM " +
                                                            "( SELECT LocationIdCode,Location, TrimedLog" +
                                                            " FROM " + table + " WHERE LogDate BETWEEN @SD AND @ED)t " +
                                                            "PIVOT(Count(LocationIdCode) for TrimedLog IN(" + Logs + "))as Pvt"
                                                         );
                            //Get the RowCount.
                            query = String.Format(
                                                    "SELECT count(*) as [RowCount]  FROM (SELECT DISTINCT Location FROM " +
                                                    table + " WHERE Election='" + electionChoice + "')as PVT"
                                                  );
                            filterFlag = true;

                        }
                        else if (electionChoice != null && (chkFilter.Checked == false))
                        {
                            // all required options selected.
                            //filter not selected.
                            queryReadLog = String.Format(
                                                            "SELECT * FROM " +
                                                            "( SELECT LocationIdCode,Location, TrimedLog" +
                                                            " FROM " + table + ")t " +
                                                            "PIVOT(Count(LocationIdCode) for TrimedLog IN(" + Logs + "))as Pvt "
                                                         );

                            //Get the RowCount.
                            query = String.Format(
                                                    "SELECT count(*) as [RowCount]  FROM (SELECT DISTINCT Location,serialnumber FROM " +
                                                     table + " WHERE Election='" + electionChoice + "' group by Location ,serialnumber)as PVT"
                                                  );
                        }
                    }
                    else if (radioSystem.Checked)
                    {
                        table = "dbo.SystemLog_" + electionDbName;
                        if (electionChoice != null && chkFilter.Checked)
                        {
                            // all required options selected.
                            //filter is selected.
                            queryReadLog = String.Format(

                                                           "SELECT * FROM " +
                                                            "( SELECT LocationIdCode,Location, [Log]" +
                                                            " FROM " + table + " WHERE LogDate BETWEEN @SD AND @ED)t " +
                                                            "PIVOT(Count(LocationIdCode) for [Log] IN(" + Logs + "))as Pvt"

                                                         );
                            //Get the RowCount.
                            query =
                                String.Format("SELECT count(*) as [RowCount]  FROM (SELECT DISTINCT Location FROM " +
                                              table + " WHERE Election='" + electionChoice + "')as Pvt");
                            filterFlag = true;
                        }
                        else if (electionChoice != null && (chkFilter.Checked == false))
                        {
                            // all required options selected.
                            //filter not selected.

                            queryReadLog = String.Format(
                                                         "SELECT * FROM " +
                                                            "( SELECT LocationIdCode,Location, [Log]" +
                                                            " FROM " + table + ")t " +
                                                            "PIVOT(Count(LocationIdCode) for [Log] IN(" + Logs + "))as Pvt"
                                                         );

                            //Get the RowCount.
                            query =
                                String.Format(
                                                "SELECT count(*) as [RowCount]  FROM (SELECT DISTINCT Location FROM " +
                                                 table + " WHERE Election='" + electionChoice + "')as Pvt"
                                              );
                        }
                    }
                    else
                    {
                        MessageBox.Show(@"Cannot Proceed. Log Choice is Empty.");
                    }

                    //command = new SqlCommand(queryReadLog, connection);


                    //Get the rowCount
                    string Sdate = sDate.ToString("yyyy-MM-dd hh:mm:ss ");
                    string Edate = eDate.ToString("yyyy-MM-dd hh:mm:ss ");
                    command = new SqlCommand(query, connection);
                    SqlDataReader rowCountReader = command.ExecuteReader();
                    if (rowCountReader != null)
                    {
                        rowCountReader.Read();
                        var rowCount = (int)rowCountReader["RowCount"];
                        rowCountReader.Close();
                        command = new SqlCommand(queryReadLog, connection);
                        command.Parameters.AddWithValue("@SD", Convert.ToDateTime(Sdate));
                        command.Parameters.AddWithValue("@ED", Convert.ToDateTime(Edate));
                        command.CommandTimeout = 72;
                        rdr = command.ExecuteReader();

                        //Get the ColumnCount.
                        int columnCount = rdr.FieldCount;
                        if (rowCount != 0)
                        {
                            Object[,] filedValues = new Object[rowCount, columnCount];
                            int i = 0;

                            //Read record reader.
                            while (rdr.Read())
                            {

                                //create worksheet column
                                for (int j = 0; j < columnCount; j++)
                                {
                                    if (rdr[j].ToString() == "" || rdr[j].ToString() == null)
                                    {
                                        filedValues[i, j] = 0;
                                    }
                                    else
                                    {
                                        filedValues[i, j] = Convert.ToString(rdr[j]);
                                    }

                                    //rdr.Read();                                 
                                } i++;

                            }

                            //Get Header from Schema.
                            int col = 0;
                            Object[,] columNames = new object[1, columnCount];
                            //getschemea.
                            DataTable schemaTable = rdr.GetSchemaTable();
                            rdr.Close();
                            if (schemaTable != null)
                                foreach (DataRow row in schemaTable.Rows)
                                {
                                    Debug.WriteLine("ColumnName={0}", row.Field<String>("ColumnName"));
                                    columNames[0, col] = row.Field<String>("ColumnName");
                                    col++;
                                }
                            //Define Range and Assign Headers.
                            m_objRange = m_objSheet.get_Range("A1", m_objOpt);
                            m_objRange = m_objRange.get_Resize(1, col);
                            m_objSheet.Name = electionChoice.Length > 29 ? electionChoice.Remove(29) : electionChoice;
                            m_objRange.Value = columNames;
                            m_objFont = m_objRange.Font;
                            m_objFont.Bold = true;
                            //Add Table Style 
                            m_objRange.Worksheet.ListObjects.Add(XlListObjectSourceType.xlSrcRange,
                            m_objRange, System.Type.Missing, XlYesNoGuess.xlYes, System.Type.Missing).Name = "Compiled";
                            m_objRange.Select();
                            m_objRange.Worksheet.ListObjects["Compiled"].TableStyle = "TableStyleMedium3";
                            //  Add array value to the worksheet starting at cell A2.
                            m_objRange = m_objSheet.get_Range("A2", m_objOpt);
                            m_objRange = m_objRange.get_Resize(rowCount, columnCount);
                            m_objRange.Value = filedValues;
                            m_objRange.EntireColumn.AutoFit();
                            //name the workbook 
                            string reportTime = DateTime.Now.ToString("HHmmss");
                            string reportName;
                            // string reportName = "Compiled Report_" + electionChoice + "_" + table + "_" + reportTime + ".xlsx";
                            if (filterFlag)
                            {
                                reportName = "Filtered_Compiled Report_" + "_" + table + "_" +
                                             reportTime + ".xlsx";
                            }
                            else
                            {
                                reportName = "Compiled Report_" + "_" + table + "_" + reportTime +
                                             ".xlsx";
                            }
                            string reportPath = string.Format("{0}{1}", m_strSampleFolder, reportName);
                            //save the workbook
                            m_objBook.SaveAs(reportPath, m_objOpt, m_objOpt,
                                m_objOpt, m_objOpt, m_objOpt, XlSaveAsAccessMode.xlNoChange,
                                m_objOpt, m_objOpt, m_objOpt, m_objOpt);
                            m_objBook.Close(false, m_objOpt, m_objOpt);
                            //Release excel objects
                            if (m_objExcel != null && Marshal.ReleaseComObject(m_objExcel) != 0)
                            {
                                Marshal.ReleaseComObject(m_objRange);
                                Marshal.ReleaseComObject(m_objSheet);
                                Marshal.ReleaseComObject(m_objSheets);
                                Marshal.ReleaseComObject(m_objBook);
                                Marshal.ReleaseComObject(m_objBooks);
                                m_objExcel.Quit();
                                Marshal.ReleaseComObject(m_objExcel);
                            }
                            //close table data reader
                            rdr.Close();
                            connection.Close();
                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                            MessageBox.Show(@"Compiled Report created at " + reportPath);
                        }
                        else
                        {
                            MessageBox.Show(@"No Records Found. Please confirm the parameters");
                            m_objBook.Close(false, m_objOpt, m_objOpt);
                            //Release excel objects
                            if (m_objExcel != null && Marshal.ReleaseComObject(m_objExcel) != 0)
                            {
                                Marshal.ReleaseComObject(m_objRange);
                                Marshal.ReleaseComObject(m_objSheet);
                                Marshal.ReleaseComObject(m_objSheets);
                                Marshal.ReleaseComObject(m_objBook);
                                Marshal.ReleaseComObject(m_objBooks);
                                m_objExcel.Quit();
                                Marshal.ReleaseComObject(m_objExcel);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex);

            }
        }

        private void SourceBtn_Click_1(object sender, EventArgs e)
        {
            //Create an instance of the FolderBrowserDialog
            this._fbd = new FolderBrowserDialog();
            if (_fbd != null)
            {
                //Instantiate new DialogResult variable
                //Purpose: To store the result of FolderBrowserDialog _fbd selection
                _result = new DialogResult();
                //Open/Display the _fdb dialog
                //Assign the return value to the variable _result
                _result = _fbd.ShowDialog();
                //Continue with the code only if 'Selected' button is pressed.
                //Otherwise (Cancel or Exit) Do nothing
                if (_result == DialogResult.OK)
                {
                    _selectedSourcePath = _fbd.SelectedPath;
                    _selectedSourcePath = Path.GetFullPath(_fbd.SelectedPath);
                    //check if there is a file with extension .hdr
                    bool exist = CheckForHeaderFile(_selectedSourcePath);
                    if (exist)
                    {
                        //set the message to display
                        _message = "Yahoo! " + Environment.NewLine + "File  with extension .hdr found!";
                        _searchPattern = "*.hdr";
                        if (_searchPattern != null && _selectedSourcePath != null)
                        {
                            string fileName = GetFileName(_selectedSourcePath, _searchPattern);
                        }
                        //Display address of the file location (address to which file is located)
                        txtSource.Text = _fbd.SelectedPath;
                    }
                    else
                    {
                        //set the message to display
                        _message = "Header File not found! " + Environment.NewLine + "Select the RIGHT Folder to proceed";
                        MessageBox.Show(_message);
                        //Wrong Selection. Reset the value of TextBox to empty, to erase any previous selections
                        //   ClearTextBox(txtSource);
                    }
                }
            }
        }

        private void btnRepDestn_Click(object sender, EventArgs e)//Folder to Create Report 
        {
            //Create an instance of the FolderBrowserDialog
            this._fbd = new FolderBrowserDialog();
            if (_fbd != null)
            {
                //Instantiate new DialogResult variable
                //Purpose: To store the result of FolderBrowserDialog _fbd selection
                _result = new DialogResult();
                //Open/Display the _fdb dialog
                //Assign the return value to the variable _result
                _result = _fbd.ShowDialog();
                //Continue with the code only if 'Selected' button is pressed.
                //Otherwise (Cancel or Exit) Do nothing
                if (_result == DialogResult.OK)
                {
                    _selectedExcelPath = Path.GetFullPath(_fbd.SelectedPath);
                    string pathStructure = Path.GetDirectoryName(_fbd.SelectedPath);
                    if (pathStructure == null)
                    {
                        txtRepDestn.Text = _selectedExcelPath;
                    }
                    else
                    {
                        txtRepDestn.Text = _selectedExcelPath + @"\";
                    }
                }
            }
        }

        private void FillCombo()//Fill copied precincts list.
        {
            try
            {
                String tableName = "dbo.Election";
                string copyflag = "1";
                var query = String.Format("select Distinct(Election) from " + tableName ,
                    tableName);
                // Create and open the connection in a using block.
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Create the Command and Parameter objects.
                    SqlCommand command = new SqlCommand(query, connection);
                    // Try to Open the connection 
                    // Create and execute the DataReader, writing the result
                    try
                    {
                        connection.Open();
                        SqlDataReader rdr = command.ExecuteReader();
                        cmbBoxElection.Items.Clear();
                        while (rdr != null && rdr.Read())
                        {
                            string electionList = rdr["Election"].ToString();                            
                            cmbBoxElection.Items.Add(electionList);
                        }
                        rdr.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(@"No Elections to Display");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Could not read the file");
            }
        }

        public bool ValidateReportGenerator()
        {
            bool flag = false;
            bool requiredValues = false;
            if (IsTextValid(txtRepDestn.Text))
            {
                if ((cmbBoxElection.SelectedIndex > -1))
                {
                    if (groupBox1.Controls.OfType<RadioButton>().Any(x => x.Checked))
                    {
                        if (radioElection.Checked)
                        {
                            repLog = "ElectionLog";
                        }
                        else if (radioSystem.Checked)
                        {
                            repLog = "SystemLog";
                        }
                        repDestn = txtRepDestn.Text;
                        repElection = cmbBoxElection.SelectedItem.ToString();
                        requiredValues = true;
                    }
                    else
                    {
                        lblvlog.Text = @"Please select Log choice";
                    }
                }
                else
                {
                    lblvelection.Text = @"Please select the Election";
                }
            }
            else
            {
                lblvdestn.Text = @"Please select a destination folder";
            }
            if (requiredValues)
            {
                if (chkFilter.Checked)
                {
                    requiredValues = false;
                    if (IsTextValid(startDate.Text))
                    {
                        repSdate = Convert.ToDateTime(startDate.Text);

                        //check if end date is valid
                        if (IsTextValid(endDate.Text))
                        {
                            if (endDate.Value < startDate.Value)
                            {
                                lblvedate.Text = @"Start Date cannot be greater than End Date.";
                            }
                            else
                            {
                                //repEdate = Convert.ToDateTime(endDate.Text);
                                repEdate = endDate.Value;
                                repEdate = Convert.ToDateTime(endDate.Text);
                                requiredValues = true;
                            }
                        }
                    }
                    else
                    {
                        lblvsdate.Text = @"Please select a start date";
                    }
                }
                else
                {
                    requiredValues = true;
                    DateTime e = DateTime.Now;
                    repEdate = e;
                    repSdate = e;
                    if (MessageBox.Show(@"Filter not selected. Report based on Election and Log will be created." + Environment.NewLine + "Proceed?", @"Attention", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        //YES 
                        requiredValues = true;
                    }
                    else
                    {
                        //NO 
                        requiredValues = false;
                    }
                }
            }
            if (requiredValues)
            {
                flag = true;
            }
            return flag;
        }

        private void btnReport_Click_1(object sender, EventArgs e)
        {
            bool createReport = ValidateReportGenerator();
            if (createReport)
            {
                UnWindUpdateTable(repElection);
                if (m_objExcel != null && Marshal.ReleaseComObject(m_objExcel) != 0)
                {
                    Marshal.ReleaseComObject(m_objExcel);
                }
                GenerateSummaryReport();
                if (m_objExcel != null && Marshal.ReleaseComObject(m_objExcel) != 0)
                {
                    Marshal.ReleaseComObject(m_objExcel);
                }
            }
        }

        private string LogChoice()
        {
            string logChoice = null;
            if (radioElection.Checked)
            {
                logChoice = "ElectionLog";
            }
            else if (radioSystem.Checked)
            {
                logChoice = "SystemLog";
            }
            return logChoice;
        }

        private void ReportValidationLabels(bool flag)
        {
            if (flag)
            {
                lblvdestn.Visible = true;
                lblvelection.Visible = true;
                lblvlog.Visible = true;
                lblvsdate.Visible = true;
            }
            else
            {
                lblvdestn.Visible = false;
                lblvelection.Visible = false;
                lblvlog.Visible = false;
                lblvsdate.Visible = false;
            }
        }

        private bool UnWindUpdateTable(string election)
        {
            String electionName = "";
            electionName = election;

            electionName = Regex.Replace(electionName, "[,]", "_");
            electionName = Regex.Replace(electionName, "[' ']", "");
            bool unWindStatus = false;
            string table = "";
            string SerialNumber = "";
            try
            {
                table = "dbo.CopiedLocations_" + electionName;
                var query = String.Format("SELECT Election,Location,LocationIdCode,SerialNumber FROM " + table + " WHERE CopyFlag='0'");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = null;
                    command = new SqlCommand(query, connection);
                    SqlDataReader rdr = command.ExecuteReader();
                    while (rdr.Read())
                    {
                        election = rdr[0] as string;
                        precinct = rdr[1] as string;
                        precinctId = rdr[2] as string;
                        SerialNumber = rdr[3] as string;

                        if (rdr != null)
                        {
                            SqlCommand command1;
                            table = "dbo.SystemLog_" + electionName;
                            var query1 =
                                String.Format("DELETE FROM " + table +
                                              " WHERE Election ='" + election + "' AND Location = '" + precinct + "' AND SerialNumber = '" + SerialNumber +"'");

                            table = "dbo.ElectionLog_" + electionName;
                            var query2 =
                                String.Format("DELETE FROM " + table +
                                              " WHERE Election ='" + election + "' AND Location = '" + precinct + "'");

                           // table = "dbo.ElectionLog_" + electionName;

                            table = "HeaderLog_" + electionName;
                            var query3 =
                                String.Format("DELETE FROM " + table +
                                              " WHERE Election ='" + election + "' AND LocationIDCode = '" +
                                              precinctId + "' AND SerialNumber = '" + SerialNumber +"'");

                            table = "dbo.CopiedLocations_" + electionName;
                            var query4 =
                                String.Format("DELETE FROM " + table +
                                              " WHERE Election ='" + election + "' AND Location = '" + precinct + "' AND SerialNumber = '" + SerialNumber + "'");

                            using (SqlConnection connection1 = new SqlConnection(connectionString))
                            {
                                connection1.Open();
                                command1 = new SqlCommand(query1, connection1);



                                //_message = @"Deleting Tables. Removing partially copied records : " +
                                //           Environment.NewLine + precinct;
                                //UpdateLabelStatus(lblStatus1, _message, Color.Blue);
                                MessageBox.Show(@"Deleting Tables. Removing partially copied records : " + Environment.NewLine + precinct);
                                command1.ExecuteNonQuery();
                                command1 = new SqlCommand(query2, connection1);
                                command1.ExecuteNonQuery();
                                command1 = new SqlCommand(query3, connection1);
                                command1.ExecuteNonQuery();
                                command1 = new SqlCommand(query4, connection1);
                                command1.ExecuteNonQuery();
                                //_message = @"Deleting Tables. Removing partially copied records : " +
                                //           Environment.NewLine + precinct;
                                //UpdateLabelStatus(lblStatus1, _message, Color.Blue);
                                MessageBox.Show(@"Removed partially copied records: " + Environment.NewLine + precinct);
                                EmptyLables();
                                FillList(election, table);
                                unWindStatus = true;
                                connection1.Close();

                            }
                        }
                        else
                        {
                            unWindStatus = false;
                        }
                    }
                    rdr.Close();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Could not read the file");
            }
            return unWindStatus;
        }
        private void startDate_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
        }
        private void endDate_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
        }
        private void chkFilter_MouseHover(object sender, EventArgs e)
        {
            // Set up the delays for the ToolTip.
            filterToolTip.AutoPopDelay = 5000;
            filterToolTip.InitialDelay = 1000;
            filterToolTip.ReshowDelay = 500;
            // Force the ToolTip text to be displayed whether or not the form is active.
            filterToolTip.ShowAlways = true;
            filterToolTip.SetToolTip(chkFilter, @"Limit Report between start date and end date");
        }
        private void txtRepDestn_TextChanged(object sender, EventArgs e)
        {
            if (IsTextValid(txtRepDestn.Text))
            {
                lblvdestn.Text = "";
            }
        }
        private void cmbBoxElection_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbBoxElection.SelectedIndex > -1)
            {
                lblvelection.Text = "";
            }
        }
        private void radioElection_CheckedChanged(object sender, EventArgs e)
        {
            if (groupBox1.Controls.OfType<RadioButton>().Any(x => x.Checked))
            {
                lblvlog.Text = "";
            }
        }
        private void radioSystem_CheckedChanged(object sender, EventArgs e)
        {
            if (groupBox1.Controls.OfType<RadioButton>().Any(x => x.Checked))
            {
                lblvlog.Text = "";
            }
        }
        private void startDate_ValueChanged(object sender, EventArgs e)
        {
            if (endDate.Value >= startDate.Value)
            {
                lblvedate.Text = "";
                lblvsdate.Text = "";
            }
        }
        private void endDate_ValueChanged(object sender, EventArgs e)
        {
            if (endDate.Value >= startDate.Value)
            {
                lblvedate.Text = "";
                lblvsdate.Text = "";
            }
        }
        private string getSerialNumber_electionLog(string electionPath)
        {
            string searchPath = electionPath + "\\log";
            string headerName = GetFilePath(searchPath, searchPattern: "Election.log");
            string[] lines = File.ReadAllLines(headerName);//Opens a text file, reads all lines of the file, and then closes the file.
            string[] value = new string[10]; //assumption: there will be only MAX 10 columns to read.
            string SerialNUmberElection = "";
            var counter = 0;
            int i = 0;
            serialNo = "";
            string getSno = null;

            serialNo = "0";
            try
            {
                if (value != null)
                {
                    foreach (string line in lines)
                    {
                        value = line.Split(',');

                        if (value[4] != " 0")
                        {
                            SerialNUmberElection = value[4];
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Could not read the file");               
               
            }//end catch
            return SerialNUmberElection;
        }
    }
}
