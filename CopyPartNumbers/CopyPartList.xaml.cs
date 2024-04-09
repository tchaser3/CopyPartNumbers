/* Title:           Copy Parts List
 * Date:            6-12-17
 * Author:          Terry Holmes */

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
using NewEventLogDLL;
using NewPartNumbersDLL;

namespace CopyPartNumbers
{
    /// <summary>
    /// Interaction logic for CopyPartList.xaml
    /// </summary>
    public partial class CopyPartList : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        PartNumberClass ThePartNumberClass = new PartNumberClass();
        OldPartsClass TheOldPartsClass = new OldPartsClass();

        //setting up the data
        OldPartNumbersDataSet TheOldPartNumbersDataSet;
        FindPartByPartIDDataSet TheFindPartByPartIDDataSet = new FindPartByPartIDDataSet();
        FindPartByPartNumberDataSet TheFindPartByPartNumberDataSet = new FindPartByPartNumberDataSet();
        FindPartByJDEPartNumberDataSet TheFindPartByJDEPartNumberDataSet = new FindPartByJDEPartNumberDataSet();

        public CopyPartList()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
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
            bool blnFatalError = false;
            float fltPrice;
            bool blnUpdatePart;
            String strTestingLocation = "THE BEGINNING";

            PleaseWait PleaseWait = new PleaseWait();
            PleaseWait.Show();

            try
            {
                //getting the record count
                intNumberOfRecords = TheOldPartNumbersDataSet.partnumbers.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    //setting up bool variable
                    blnUpdatePart = false;

                    //loading variables
                    intPartID = TheOldPartNumbersDataSet.partnumbers[intCounter].PartID;
                    strPartNumber = TheOldPartNumbersDataSet.partnumbers[intCounter].PartNumber;
                    strPartDescription = TheOldPartNumbersDataSet.partnumbers[intCounter].Description;
                    strJDEPartNumber = TheOldPartNumbersDataSet.partnumbers[intCounter].JDEPartNumber;
                    fltPrice = float.Parse(Convert.ToString(TheOldPartNumbersDataSet.partnumbers[intCounter].Price));

                    TheFindPartByPartIDDataSet = ThePartNumberClass.FindPartByPartID(intPartID);

                    intRecordsReturned = TheFindPartByPartIDDataSet.FindPartByPartID.Rows.Count;

                    strTestingLocation = "ONE";

                    if (intRecordsReturned == 0)
                    {
                        TheFindPartByPartNumberDataSet = ThePartNumberClass.FindPartByPartNumber(strPartNumber);

                        if(intRecordsReturned == 0)
                        {
                            if((strJDEPartNumber != "NOT REQUIRED") && (strJDEPartNumber != "NOT PROVIDED"))
                            {
                                TheFindPartByJDEPartNumberDataSet = ThePartNumberClass.FindPartByJDEPartNumber(strJDEPartNumber);

                                intRecordsReturned = TheFindPartByJDEPartNumberDataSet.FindPartByJDEPartNumber.Rows.Count;

                                if (intRecordsReturned > 0)
                                {
                                    strTestingLocation = "TWO";

                                    if (strPartDescription != TheFindPartByJDEPartNumberDataSet.FindPartByJDEPartNumber[0].PartDescription)
                                    {
                                        blnUpdatePart = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            strTestingLocation = "THREE";
                            
                            if (strPartDescription != TheFindPartByPartIDDataSet.FindPartByPartID[0].PartDescription)
                            {
                                blnUpdatePart = true;
                            }
                            if (strJDEPartNumber != TheFindPartByPartIDDataSet.FindPartByPartID[0].JDEPartNumber)
                            {
                                blnUpdatePart = true;
                            }
                        }
                    }
                    else
                    {
                        strTestingLocation = "FOUR";

                        if (strPartDescription != TheFindPartByPartIDDataSet.FindPartByPartID[0].PartDescription)
                        {
                            blnUpdatePart = true;
                        }
                        if (strJDEPartNumber != TheFindPartByPartIDDataSet.FindPartByPartID[0].JDEPartNumber)
                        {
                            blnUpdatePart = true;
                        }
                    }

                    if(intRecordsReturned == 0)
                    {
                        blnFatalError = ThePartNumberClass.InsertPartIntoPartNumbers(intPartID, strPartNumber, strJDEPartNumber, strPartDescription, fltPrice);

                        if(blnFatalError == true)
                        {
                            TheMessagesClass.ErrorMessage("Contact IT");
                            return;
                        }
                    }
                    else if(blnUpdatePart == true)
                    {
                        blnFatalError = ThePartNumberClass.UpdatePartInformation(intPartID, strJDEPartNumber, strPartDescription, true, fltPrice);

                        if (blnFatalError == true)
                        {
                            TheMessagesClass.ErrorMessage("Contact IT");
                            return;
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Copy Part Numbers // Copy Part List // Process Button " + strTestingLocation + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

            PleaseWait.Close();
        }

        private void btnMainMenu_Click(object sender, RoutedEventArgs e)
        {
            MainMenu MainMenu = new MainMenu();
            MainMenu.Show();
            Close();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                TheOldPartNumbersDataSet = TheOldPartsClass.GetOldPartNumbersInfo();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Copy Part Numbers // Copy Part List // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
