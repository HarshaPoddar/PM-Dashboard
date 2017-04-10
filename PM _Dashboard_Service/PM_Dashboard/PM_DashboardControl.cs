using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;
using System.Data;
using System.Text.RegularExpressions;
using System.ComponentModel;
using PMDashboard.BusinessLogic;
using Microsoft.TeamFoundation;

namespace PM_Dashboard
{
    public partial class PM_DashboardControl : UserControl
    {
        #region Global Variables
        //Adding excel workbook and sheets
        private static Excel.Application xlApp = Globals.ThisAddIn.Application;
        Excel.Workbook xlWorkBook = xlApp.ActiveWorkbook;
        Excel.Worksheet xlWorkSheetPg1;
        Excel.Worksheet xlWorkSheetPg2;
        Excel.Worksheet xlWorkSheetPg3;
        Excel.Worksheet xlWorkSheetPg4;
        Excel.Worksheet xlWorkSheetPg5;
        object misValue = System.Reflection.Missing.Value;
        private static List<TeamMember> tbsHoursBurnt = new List<TeamMember>();
        private static EncryptAndDecrypt encrypt = new EncryptAndDecrypt();
        TfSDetails tfsdetails;
        IPMDashboardBusinessLogic businessLogic = new TFS();
        //private static string cipherData;

        #endregion

        public PM_DashboardControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Setting Visibility and Getting the List of Projects from TFS Server and Binding to ComboBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void GetProjectsList(object sender, EventArgs e)
        {  
            try
            {
                #region Sheet Validation

                if (xlWorkSheetPg5.Cells[2, 2].Value != null)
                {
                    DialogResult dialogResult = MessageBox.Show("Warning! Contents will be deleted", "Attention!", MessageBoxButtons.OK);
                    if (dialogResult == DialogResult.OK)
                    {
                        xlApp = Globals.ThisAddIn.Application;
                        xlWorkBook = xlApp.ActiveWorkbook;
                        xlWorkBook.Sheets[1].Cells.ClearContents();
                        xlWorkBook.Sheets[2].Cells.ClearContents();
                        xlWorkBook.Sheets[3].Delete();
                        xlWorkSheetPg3 = (Excel.Worksheet)xlWorkBook.Sheets.Add(After: xlWorkBook.Sheets[2]);
                        xlWorkBook.Sheets[4].Cells.ClearContents();
                        xlWorkBook.Sheets[5].Cells.ClearContents();
                    }
                }

                #endregion

                Browse.Visible = false;
                SelectTBSFile.Visible = false;
                SelectProjectLabel.Visible = false;
                cmbProjectList.Visible = false;
                ViewDetails.Visible = false;

                //Declare a dictionary to hold the list of projects
                Dictionary<string, string> ListOfProjects = new Dictionary<string, string>();
                //Check the validation of text boxes
                if (ValidateChildren(ValidationConstraints.None))
                {
                    // Get the user Credentials
                    tfsdetails = new TfSDetails();
                    tfsdetails.CollectionsURI = new Uri(txtServerLink.Text);
                    tfsdetails.PassWord = txtPassword.Text;
                    tfsdetails.UserName = txtUserName.Text;

                    #region Adding the list of projects to the combo box
                    //Get List Of Projects 

                    ListOfProjects = await businessLogic.GetProjectList(tfsdetails);

                    #region Configuration Sheet
                    xlWorkSheetPg5.Cells[1, 1] = "Configuration Sheet";
                    xlWorkSheetPg5.Cells[1, 1].Font.Bold = true;
                    xlWorkSheetPg5.Cells[2, 1] = "Server Link";
                    xlWorkSheetPg5.Cells[2, 1].Font.Bold = true;
                    xlWorkSheetPg5.Cells[3, 1] = "User Name";
                    xlWorkSheetPg5.Cells[3, 1].Font.Bold = true;
                    xlWorkSheetPg5.Cells[4, 1] = "Password";
                    xlWorkSheetPg5.Cells[4, 1].Font.Bold = true;
                    xlWorkSheetPg5.Name = "Configuration Sheet";
                    #endregion

                    
                        cmbProjectList.DataSource = new BindingSource(ListOfProjects, null);
                        cmbProjectList.DisplayMember = "Value";
                        cmbProjectList.ValueMember = "Key";
                    
                    //Making the other buttons visible.
                    SelectProjectLabel.Visible = true;
                    cmbProjectList.Visible = true;
                    ViewDetails.Visible = false;
                    Browse.Visible = true;
                    SelectTBSFile.Visible = true;
                    xlWorkSheetPg5.Cells[2, 2] = txtServerLink.Text;
                    xlWorkSheetPg5.Cells[3, 2] = txtUserName.Text;
                    xlWorkSheetPg5.Cells[4, 2] = encrypt.Encrypt(txtPassword.Text);
                    CredentailsValidation.Clear();

                    #endregion

                    ((Excel.Worksheet)xlApp.ActiveWorkbook.Sheets[5]).Select();
                }
            }
            catch (TeamFoundationServerUnauthorizedException ex)
            {
                CredentailsValidation.SetError(GetProjectButton, "Please enther valid credentials");
            }
            catch (TeamFoundationServiceUnavailableException ex)
            {
                MessageBox.Show("Please connect To TFS and try again", "ServerError", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Environment.Exit(666);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "ServerError", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Environment.Exit(666);
            }
        }
     

        /// <summary>
        /// Setting Visibility after Form Loads
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PM_DashboardControl_Load(object sender, EventArgs e)
        {

            cmbProjectList.Visible = false;
            SelectProjectLabel.Visible = false;
            ViewDetails.Visible = false;
            Browse.Visible = false;
            SelectTBSFile.Visible = false;

            #region Validating Number of Sheets
            int noOfSheets = xlWorkBook.Sheets.Count;
            ValidateSheets(noOfSheets);
            #endregion

        }

        /// Gets the Sprint Details of the Selected Project from the Project List.
        /// Populates the Sprint Details on Excel and generates charts on excel for Sprint Details.
        /// Populates the TFS vs TBS Details on Excel and generates charts based on the data populated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ViewDetails_Click(object sender, EventArgs e)
        {
            if (cmbProjectList.Text.Count() != 0)
            {
                SelectProjectErrorProvider.Clear();
                try
                {
                    if (xlWorkSheetPg1.Cells[2, 2].Value != null)
                    {
                        MessageBox.Show("Details already populated! Please see the sheets.", "Attention!", MessageBoxButtons.OK);
                        return;
                    }
                    if (ValidateChildren(ValidationConstraints.None))
                    {
                        #region Initializing the credential to the object
                        KeyValuePair<string, string> selectedProject = (KeyValuePair<string, string>)cmbProjectList.SelectedItem;
                        tfsdetails = new TfSDetails()
                        {
                            UserName = txtUserName.Text,
                            PassWord = txtPassword.Text,
                            CollectionsURI = new Uri(txtServerLink.Text),
                            ProjectURI = selectedProject.Key,
                            ProjectName = selectedProject.Value
                        };

                        List<SprintDetails> sprintDetails;
                        #endregion

                        #region Populating Sprint Details and Generating chart for the Sprints.
                        //Getting the Sprint Details Chart based on the Sprint Values.

                        sprintDetails = await businessLogic.GetSprintDetails(tfsdetails);

                        List<string> sprintHeader = new List<string>() { "Planned", "Remaining", "Capacity", "startDate", "endDate" };


                        Excel.Range chartRange;
                        Excel.ChartObjects xlCharts = (Excel.ChartObjects)xlWorkSheetPg3.ChartObjects(Type.Missing);
                        Excel.Chart Sprint1Chart = xlCharts.Add(0, 0, 500, 250).Chart;

                        Sprint1Chart.HasTitle = true;
                        Sprint1Chart.ChartTitle.Text = string.Format("{0} - Sprint Details", selectedProject.Value);
                        var yAxis = (Excel.Axis)Sprint1Chart.Axes(Excel.XlAxisType.xlValue, Excel.XlAxisGroup.xlPrimary);
                        yAxis.HasTitle = true;
                        yAxis.AxisTitle.Text = "Hours";

                        //add sprint data to sheet2
                        int columnSprint = 2;
                        int columnIndex = 0;
                        foreach (var item in sprintDetails)
                        {
                            int rowSprint = 3;
                            xlWorkSheetPg1.Cells[1, 1] = string.Format("{0} - Sprint Details", selectedProject.Value);
                            xlWorkSheetPg1.Cells[1, 1].Font.Bold = true;
                            xlWorkSheetPg1.Cells[2, columnSprint] = item.SprintName;
                            xlWorkSheetPg1.Cells[2, columnSprint].Font.Bold = true;
                            xlWorkSheetPg1.Cells[rowSprint++, columnSprint].Value = item.planned;
                            xlWorkSheetPg1.Cells[rowSprint++, columnSprint].Value = item.remaining;
                            xlWorkSheetPg1.Cells[rowSprint++, columnSprint].Value = item.capacity;
                            columnIndex = columnSprint + 1;
                            xlWorkSheetPg1.Cells[rowSprint++, columnSprint].Value = item.startDate;
                            xlWorkSheetPg1.Columns.AutoFit();
                            xlWorkSheetPg1.Cells[rowSprint++, columnSprint++].Value = item.endDate;
                            xlWorkSheetPg1.Columns.AutoFit();
                        }
                        //Adding the header for the sprint details
                        int counter = 3;
                        foreach (var item in sprintHeader)
                        {
                            xlWorkSheetPg1.Cells[counter, 1] = item;
                            xlWorkSheetPg1.Cells[counter++, 1].Font.Bold = true;

                        }
                        //Chart For Sprint Details
                        string ColumnName = GetExcelColumnName(columnIndex) + "5";
                        chartRange = xlWorkSheetPg1.get_Range("A2", ColumnName);
                        Sprint1Chart.SetSourceData(chartRange, misValue);
                        Sprint1Chart.ChartType = Excel.XlChartType.xlColumnClustered;
                        List<TeamMember> tfsHoursBurnt = new List<TeamMember>();
                        xlWorkSheetPg1.Name = "Sprint Details";
                        #endregion

                        #region Populating TFS vs TBS Details and Generating charts for each TeamMember

                        tfsdetails.UserName = txtUserName.Text;
                        tfsHoursBurnt = await businessLogic.TeamMemberDetails(tfsdetails);

                        if (tfsHoursBurnt != null)
                        {
                            List<string> TeamChartheader = new List<string>() { "Date", "TBS", "TFS" };

                            //add TFS vs TFS data to sheet2
                            columnSprint = 2;
                            columnIndex = 1;
                            counter = 1;
                            int chartTopLocation = 10;
                            foreach (var tfsData in tfsHoursBurnt)
                            {
                                Excel.Range TBSchartRange;
                                Excel.ChartObjects TBSChart = (Excel.ChartObjects)xlWorkSheetPg3.ChartObjects(Type.Missing);
                                Excel.ChartObject myTBSChart = TBSChart.Add(0, chartTopLocation += 250, 500, 250);
                                Excel.Chart TFSvTBSChart = myTBSChart.Chart;
                                TFSvTBSChart.HasTitle = true;
                                TFSvTBSChart.ChartTitle.Text = tfsData.Name;
                                var yAxisForSprintDetailsChart = (Excel.Axis)TFSvTBSChart.Axes(Excel.XlAxisType.xlValue, Excel.XlAxisGroup.xlPrimary);
                                yAxisForSprintDetailsChart.HasTitle = true;
                                yAxisForSprintDetailsChart.AxisTitle.Text = "Hours Burnt";

                                int teamRow = 4;
                                Dictionary<int, TeamMember> ComparisonResult = new Dictionary<int, TeamMember>();
                                foreach (var tbsData in tbsHoursBurnt)
                                {
                                    int compare = LevenshteinDistance.Compute(tfsData.Name.Trim(), tbsData.Name.Trim());
                                    if (!ComparisonResult.ContainsKey(compare))
                                        ComparisonResult.Add(compare, tbsData);
                                }
                                var tbsFinalMatch = ComparisonResult[ComparisonResult.Keys.Min()];

                                xlWorkSheetPg2.Cells[2, columnSprint].Value = tfsData.Name;
                                xlWorkSheetPg2.Cells[2, columnSprint].Font.Bold = true;
                                columnSprint += 3;

                                foreach (var header in TeamChartheader)
                                {
                                    xlWorkSheetPg2.Cells[3, columnIndex] = header;
                                    xlWorkSheetPg2.Cells[3, columnIndex++].Font.Bold = true;

                                }
                                int columnChartEndRange = 0;
                                int columnChartStartRange = 0;
                                int length = 3;
                                foreach (KeyValuePair<DateTime?, double?> entry in tfsData.HoursBurnt)
                                {
                                    xlWorkSheetPg2.Cells[teamRow, counter++] = entry.Key;
                                    xlWorkSheetPg2.Columns.AutoFit();
                                    if (tbsFinalMatch.HoursBurnt.ContainsKey(entry.Key))
                                        xlWorkSheetPg2.Cells[teamRow, counter++] = tbsFinalMatch.HoursBurnt[entry.Key];
                                    else
                                        xlWorkSheetPg2.Cells[teamRow, counter++] = tbsFinalMatch.HoursBurnt.Values.FirstOrDefault();
                                    xlWorkSheetPg2.Cells[teamRow++, counter] = entry.Value;
                                    columnChartEndRange = counter;
                                    columnChartStartRange = counter - 2;
                                    length++;
                                    counter -= 2;
                                }
                                counter += 3;

                                string columnEndRange = GetExcelColumnName(columnChartEndRange) + length;
                                string columnStartRange = GetExcelColumnName(columnChartStartRange) + 3;

                                TBSchartRange = xlWorkSheetPg2.get_Range(columnStartRange, columnEndRange);
                                TFSvTBSChart.SetSourceData(TBSchartRange, misValue);
                                TFSvTBSChart.ChartType = Excel.XlChartType.xlColumnClustered;

                                xlWorkSheetPg2.Name = "TBS vs TFS Data";
                                xlWorkSheetPg3.Name = "Charts";

                            }
                            #endregion

                            #region Hide Extra date columns
                            if (xlWorkSheetPg2.Columns.Count > 3)
                            {
                                for (int index = 4; index < xlWorkSheetPg2.Columns.Count; index = index + 3)
                                {
                                    xlWorkSheetPg2.Columns[index].ColumnWidth = 0.1;
                                }
                            }
                            #endregion

                        ((Excel.Worksheet)xlApp.ActiveWorkbook.Sheets[3]).Select();
                        }
                        else
                            MessageBox.Show("No Team Members Found");
                    }
                   
                }
                catch (TeamFoundationServerUnauthorizedException ex)
                {
                    CredentailsValidation.SetError(GetProjectButton, "Please enther valid credentials");
                }
                catch (TeamFoundationServiceUnavailableException ex)
                {
                    MessageBox.Show("Please connect To TFS and try again", "ServerError", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    Environment.Exit(666);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "ServerError", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    Environment.Exit(666);
                }
            }
            else
            {
                SelectProjectErrorProvider.SetError(cmbProjectList, "Cannot be Null! Please choose from the list");
            }
        }
        

        /// <summary>
        /// Releasing the excel objects
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PM_DashboardControl_Leave(object sender, EventArgs e)
        {
            #region ReleaseObject
            releaseObject(xlWorkSheetPg1);
            releaseObject(xlWorkSheetPg2);
            releaseObject(xlWorkSheetPg3);
            releaseObject(xlWorkSheetPg4);
            releaseObject(xlWorkSheetPg5);
            releaseObject(xlWorkBook);
            releaseObject(xlApp);
            #endregion
        }

        /// <summary>
        /// Clean up of Excel interop objects.
        /// </summary>
        /// <param name="obj"></param>
        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                MessageBox.Show("Exception Occured while releasing object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }

        }

        /// <summary>
        /// Opening and populating the TBS file on Excel and Reading for TBS data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Browse_Click(object sender, EventArgs e)
        {
            if (cmbProjectList.Text.Count() != 0)
            {
                SelectProjectErrorProvider.Clear();
                xlWorkSheetPg4.Cells.ClearContents();
                DialogResult result = openFileDialog1.ShowDialog();
                xlWorkSheetPg4.Name = "TBS Data";

                #region Opening csv File and populating on Excel
               
                    if (result == DialogResult.OK) // Test result.
                    {
                        string file = openFileDialog1.FileName;
                        string test = Path.GetExtension(file);
                        var length = new FileInfo(file).Length;

                        #region Validating the given csv file
                        if (IsFileLocked(file) == false)
                        {
                            if (length != 0 && length != 2)
                            {
                                if (Path.GetExtension(file) == ".csv")
                                {
                                    browseErrorprovider.Clear();

                                    List<string> lines = new List<string>();
                                    // var csvLines = File.ReadAllLines(file).Select(a => a.Split('\t'));

                                    using (StreamReader reader = new StreamReader(file))
                                    {
                                        string line;
                                        while ((line = reader.ReadLine()) != null)
                                        {
                                            lines.Add(line);
                                        }
                                    }
                                    int rowIndex = 1;
                                    int ColumnIndex = 1;
                                    xlWorkSheetPg4 = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(4);

                                    foreach (String line in lines)
                                    {
                                        var values = line.Split('\t');
                                        foreach (String value in values)
                                        {
                                            xlWorkSheetPg4.Cells[rowIndex, ColumnIndex++] = value;
                                        }
                                        rowIndex++;
                                        ColumnIndex = 1;
                                    }

                                    #region Reading the csv file to get the TBS Data
                                    int Counter = 0;
                                    int blankCounter = 0;

                                    for (int tbsRow = 1; tbsRow < xlWorkSheetPg4.Rows.Count; tbsRow++)
                                    {
                                        if (Convert.ToString(xlWorkSheetPg4.Cells[tbsRow, 3].Value) == "Billable" && blankCounter < 20)
                                        {
                                            browseErrorprovider.Clear();
                                            Counter++;
                                            blankCounter = 0;
                                            string teamMemberName = xlWorkSheetPg4.Cells[tbsRow, 1].Value;
                                            teamMemberName = teamMemberName.Substring(1);
                                            DateTime tbsBillingDate = xlWorkSheetPg4.Cells[tbsRow, 5].Value;
                                            double tbsBilledHours = xlWorkSheetPg4.Cells[tbsRow, 6].Value;

                                            if (!tbsHoursBurnt.Any(c => c.Name == teamMemberName))
                                            {
                                                tbsHoursBurnt.Add(new TeamMember()
                                                {
                                                    Name = teamMemberName,
                                                    HoursBurnt = new Dictionary<Nullable<DateTime>, Nullable<double>>()
                                                });
                                            }

                                            var tbsForUser = tbsHoursBurnt.Where(c => c.Name == teamMemberName).FirstOrDefault();

                                            if (!tbsForUser.HoursBurnt.ContainsKey(tbsBillingDate))
                                                tbsForUser.HoursBurnt.Add(tbsBillingDate, tbsBilledHours);
                                            else
                                                tbsForUser.HoursBurnt[tbsBillingDate] += tbsBilledHours;
                                            ((Excel.Worksheet)xlApp.ActiveWorkbook.Sheets[4]).Select();
                                        }
                                        else if (string.IsNullOrEmpty((Convert.ToString(xlWorkSheetPg4.Cells[tbsRow, 3].Value))))
                                            blankCounter++;
                                        if (blankCounter >= 20)
                                            break;
                                    }

                                    if (Counter == 0)
                                    {
                                        xlWorkSheetPg4.Cells.ClearContents();
                                        browseErrorprovider.SetError(Browse, "No Billable column found! Please check the file.");
                                    }
                                    #endregion
                                    if (cmbProjectList.Text.Count() == 0)
                                        SelectProjectErrorProvider.SetError(cmbProjectList, "Cannot be Null! Please choose from the list");
                                    else
                                        SelectProjectErrorProvider.Clear();
                                    ViewDetails.Visible = true;
                                }
                                else
                                {
                                    browseErrorprovider.SetError(Browse, "Please Select File Of Type .csv");
                                }
                            }
                            else
                            {
                                browseErrorprovider.SetError(Browse, "Empty CSV file. Please choose the correct file");
                            }
                        }
                        else
                        {
                            browseErrorprovider.SetError(Browse, "File Open in another process! please close and try again.");
                        }
                    }
                    #endregion
                }
                else
                {
                    SelectProjectErrorProvider.SetError(cmbProjectList, "Cannot be Null! Please choose from the list");
                }
                #endregion
        }

        /// <summary>
        /// Get the Column Name by the index
        /// </summary>
        /// <param name="columnNumber">the index of the column.</param>
        /// <returns>The Name of the Column corresponding to the index.</returns>
        private string GetExcelColumnName(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = String.Empty;
            int modulo;
            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }
            return columnName;
        }

        #region Validation
        private void PasswordTextbox_Validating(object sender, CancelEventArgs getprojectevent)
        {
            if (string.IsNullOrEmpty(txtPassword.Text) && (xlWorkSheetPg5.Cells[4, 2].Value == null))
            {
                getprojectevent.Cancel = true;

                PassworderrorProvider.SetError(txtPassword, "Password required!");
            }
            else if (string.IsNullOrEmpty(txtPassword.Text) && xlWorkSheetPg5.Cells[4, 2].Value != null)
            {

                string Password = encrypt.Decrypt();
                txtPassword.Text = Password;
            }
            else if (!Regex.IsMatch(txtPassword.Text, @"[A-Za-z][A-Za-z0-9]{2,7}"))
            {
                getprojectevent.Cancel = true;

                PassworderrorProvider.SetError(txtPassword, "Password invalid!");
            }
            else
            {
                getprojectevent.Cancel = false;

                PassworderrorProvider.Clear();

            }

        }

        private void UserNameTextbox_Validating(object sender, CancelEventArgs getprojectevent)
        {
            if (string.IsNullOrEmpty(txtUserName.Text) && (xlWorkSheetPg5.Cells[3, 2].Value == null))
            {
                getprojectevent.Cancel = true;

                userNameErrorProvider.SetError(txtUserName, "User name required!");
            }
            else if (string.IsNullOrEmpty(txtUserName.Text) && xlWorkSheetPg5.Cells[3, 2].Value != null)
            {
                txtUserName.Text = xlWorkSheetPg5.Cells[3, 2].Value;
            }

            else if (txtUserName.Text != null && !txtUserName.Text.Contains('/'))
            {
                getprojectevent.Cancel = true;
                userNameErrorProvider.SetError(txtUserName, "Please Enter the correct Domain Name.");
            }
            else
            {
                getprojectevent.Cancel = false;
                userNameErrorProvider.Clear();
            }
        }

        private void ServerLinkTextbox_Validating(object sender, CancelEventArgs getprojectevent)
        {

            if (string.IsNullOrEmpty(txtServerLink.Text) && (xlWorkSheetPg5.Cells[2, 2].Value == null))
            {
                getprojectevent.Cancel = true;
                txtServerLink.Focus();
                serverLinkErrorProvider.SetError(txtServerLink, "ServerLinkRequired!");
            }
            else if (string.IsNullOrEmpty(txtServerLink.Text) && xlWorkSheetPg5.Cells[2, 2].Value != null)
            {
                txtServerLink.Text = xlWorkSheetPg5.Cells[2, 2].Value;
            }
            else if (txtServerLink.Text.Substring(0, 4) != Uri.UriSchemeHttp)
            {
                getprojectevent.Cancel = true;
                txtServerLink.Focus();
                serverLinkErrorProvider.SetError(txtServerLink, "Enter A Valid Url!");
            }
            else
            {
                getprojectevent.Cancel = false;
                serverLinkErrorProvider.Clear();
            }
        }

        private void SelectProjectListbox_Validating(object sender, CancelEventArgs getprojectevent)
        {

        }

        /// <summary>
        /// Validating the number of sheets.
        /// </summary>
        /// <param name="NumberOfSheets">the no of sheets in the workbook</param>
        private void ValidateSheets(int NumberOfSheets)
        {
            if (NumberOfSheets == 0)
            {
                xlWorkSheetPg1 = (Excel.Worksheet)xlWorkBook.Sheets.Add(After: xlWorkBook.Sheets[0]);
                xlWorkSheetPg2 = (Excel.Worksheet)xlWorkBook.Sheets.Add(After: xlWorkBook.Sheets[1]);
                xlWorkSheetPg3 = (Excel.Worksheet)xlWorkBook.Sheets.Add(After: xlWorkBook.Sheets[2]);
                xlWorkSheetPg4 = (Excel.Worksheet)xlWorkBook.Sheets.Add(After: xlWorkBook.Sheets[3]);
                xlWorkSheetPg5 = (Excel.Worksheet)xlWorkBook.Sheets.Add(After: xlWorkBook.Sheets[4]);
            }
            if (NumberOfSheets == 1)
            {
                xlWorkSheetPg2 = (Excel.Worksheet)xlWorkBook.Sheets.Add(After: xlWorkBook.Sheets[1]);
                xlWorkSheetPg3 = (Excel.Worksheet)xlWorkBook.Sheets.Add(After: xlWorkBook.Sheets[2]);
                xlWorkSheetPg4 = (Excel.Worksheet)xlWorkBook.Sheets.Add(After: xlWorkBook.Sheets[3]);
                xlWorkSheetPg5 = (Excel.Worksheet)xlWorkBook.Sheets.Add(After: xlWorkBook.Sheets[4]);
            }
            else if (NumberOfSheets == 2)
            {
                xlWorkSheetPg3 = (Excel.Worksheet)xlWorkBook.Sheets.Add(After: xlWorkBook.Sheets[2]);
                xlWorkSheetPg4 = (Excel.Worksheet)xlWorkBook.Sheets.Add(After: xlWorkBook.Sheets[3]);
                xlWorkSheetPg5 = (Excel.Worksheet)xlWorkBook.Sheets.Add(After: xlWorkBook.Sheets[4]);
            }
            else if (NumberOfSheets == 3)
            {
                xlWorkSheetPg4 = (Excel.Worksheet)xlWorkBook.Sheets.Add(After: xlWorkBook.Sheets[3]);
                xlWorkSheetPg5 = (Excel.Worksheet)xlWorkBook.Sheets.Add(After: xlWorkBook.Sheets[4]);
            }
            else if (NumberOfSheets == 4)
            {
                xlWorkSheetPg5 = (Excel.Worksheet)xlWorkBook.Sheets.Add(After: xlWorkBook.Sheets[4]);
            }

            xlWorkSheetPg1 = (Excel.Worksheet)this.xlWorkBook.Worksheets.get_Item(1);
            xlWorkSheetPg2 = (Excel.Worksheet)this.xlWorkBook.Worksheets.get_Item(2);
            xlWorkSheetPg3 = (Excel.Worksheet)this.xlWorkBook.Worksheets.get_Item(3);
            xlWorkSheetPg4 = (Excel.Worksheet)this.xlWorkBook.Worksheets.get_Item(4);
            xlWorkSheetPg5 = (Excel.Worksheet)this.xlWorkBook.Worksheets.get_Item(5);
        }

        public bool IsFileLocked(string filename)
        {
            bool Locked = false;
            try
            {
                FileStream fs =
                    File.Open(filename, FileMode.OpenOrCreate,
                    FileAccess.ReadWrite, FileShare.None);
                fs.Close();
            }
            catch (IOException ex)
            {
                Locked = true;
            }
            return Locked;
        }

        #endregion

    }
}