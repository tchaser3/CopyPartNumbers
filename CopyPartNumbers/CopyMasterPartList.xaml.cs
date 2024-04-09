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
using System.Windows.Shapes;
using NewPartNumbersDLL;
using NewEventLogDLL;

namespace CopyPartNumbers
{
    /// <summary>
    /// Interaction logic for CopyMasterPartList.xaml
    /// </summary>
    public partial class CopyMasterPartList : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        PartNumberClass ThePartNumberClass = new PartNumberClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        OldPartsClass TheOldPartsClass = new OldPartsClass();

        //setting up the data
        OldMasterPartsListDataSet TheOldMasterPartsListDataSet;
        FindPartFromMasterPartListByPartNumberDataSet TheFindPartFromMasterPartListByPartNumberDataSet = new FindPartFromMasterPartListByPartNumberDataSet();
        FindPartFromMasterPartListByJDEPartNumberDataSet TheFindPartFromMasterPartListByJDEPartNumberDataSet = new FindPartFromMasterPartListByJDEPartNumberDataSet();
        
        public CopyMasterPartList()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void btnMainMenu_Click(object sender, RoutedEventArgs e)
        {
            MainMenu MainMenu = new MainMenu();
            MainMenu.Show();
            Close();
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            int intCounter;
            int intNumberOfRecords;
            int intRecordsReturned;
            int intPartID;
            string strPartNumber;
            string strJDEPartNumber;
            string strPartDescription;
            float fltPrice;
            bool blnFatalError;

            PleaseWait PleaseWait = new PleaseWait();
            PleaseWait.Show();

            try
            {
                //getting the count
                intNumberOfRecords = TheOldMasterPartsListDataSet.masterpartlist.Rows.Count - 1;

                //lop\op
                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    intPartID = TheOldMasterPartsListDataSet.masterpartlist[intCounter].PartID;
                    strPartNumber = TheOldMasterPartsListDataSet.masterpartlist[intCounter].PartNumber;
                    strJDEPartNumber = TheOldMasterPartsListDataSet.masterpartlist[intCounter].JDEPartNumber;
                    strPartDescription = TheOldMasterPartsListDataSet.masterpartlist[intCounter].Description;
                    fltPrice = float.Parse(Convert.ToString(TheOldMasterPartsListDataSet.masterpartlist[intCounter].Price));

                    TheFindPartFromMasterPartListByPartNumberDataSet = ThePartNumberClass.FindPartFromMasterPartListByPartNumber(strPartNumber);

                    intRecordsReturned = TheFindPartFromMasterPartListByPartNumberDataSet.FindPartFromMasterPartListByPartNumber.Rows.Count;

                    if(intRecordsReturned == 0)
                    {
                        if ((strJDEPartNumber != "NOT REQUIRED") && (strJDEPartNumber != "NOT PROVIDED"))
                        {
                            TheFindPartFromMasterPartListByJDEPartNumberDataSet = ThePartNumberClass.FindPartFromMasterPartListByJDEPartNumber(strPartNumber);

                            intRecordsReturned = TheFindPartFromMasterPartListByJDEPartNumberDataSet.FindPartFromMasterPartListByJDEPartNumber.Rows.Count;
                        }
                    }

                    if(intRecordsReturned == 0)
                    {
                        blnFatalError = ThePartNumberClass.InsertPartIntoMasterPartList(intPartID, strPartNumber, strJDEPartNumber, strPartDescription, fltPrice);

                        if(blnFatalError == true)
                        {
                            TheMessagesClass.ErrorMessage("Contact IT");
                            return;
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Copy Part Numbers // Copy Master Part List // Process Button " + Ex.Message);
            }

            PleaseWait.Close();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //this will load up the data
            try
            {
                TheOldMasterPartsListDataSet = TheOldPartsClass.GetOldMasterPartsList();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Copy Part Numbers // Copy Master Part List // Window Loaded " + Ex.Message);
            }
        }
    }
}
