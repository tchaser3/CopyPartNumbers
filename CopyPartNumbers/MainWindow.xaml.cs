/* Title:           Copy Part Numbers
 * Date:            6-9-17
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using NewEmployeeDLL;
using NewEventLogDLL;
using DataValidationDLL;

namespace CopyPartNumbers
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();

        public static VerifyLogonDataSet TheVerifyLogonDataSet = new VerifyLogonDataSet();

        int gintNoOfMisses;

        public MainWindow()
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

        private void btnSignIn_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            int intEmployeeID = 0;
            string strLastName;
            string strErrorMessage = "";
            string strValueForValidation;
            bool blnFatalError;
            int intRecordsReturned;

            //beginning data validation
            strLastName = txtLastName.Text;
            strValueForValidation = pbxEmployeeID.Password.ToString();
            blnFatalError = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
            if(blnFatalError == true)
            {
                strErrorMessage += "The Employee ID Entered is not an Integer\n";
            }
            else
            {
                intEmployeeID = Convert.ToInt32(strValueForValidation);
            }
            if(strLastName == "")
            {
                blnFatalError = true;
                strErrorMessage += "The Last Name Was Not Entered\n";
            }
            if(blnFatalError == true)
            {
                TheMessagesClass.ErrorMessage(strErrorMessage);
                return;
            }

            //getting data
            TheVerifyLogonDataSet = TheEmployeeClass.VerifyLogon(intEmployeeID, strLastName);

            //gettign a record count
            intRecordsReturned = TheVerifyLogonDataSet.VerifyLogon.Rows.Count;

            if(intRecordsReturned == 0)
            {
                LogonFailed();
            }
            else
            {
                if(TheVerifyLogonDataSet.VerifyLogon[0].EmployeeGroup != "ADMIN")
                {
                    LogonFailed();
                }
                else
                {
                    MainMenu MainMenu = new MainMenu();
                    MainMenu.Show();
                    Hide();
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            gintNoOfMisses = 0;

            pbxEmployeeID.Focus();
        }
        private void LogonFailed()
        {
            gintNoOfMisses++;

            if(gintNoOfMisses == 3)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "There Have Been Three Attempts To Sign In the Copy Part Number Program");

                TheMessagesClass.ErrorMessage("You Have Attempted Three Times to Sign In, The Program Will Close");

                Application.Current.Shutdown();
            }
            else
            {
                TheMessagesClass.InformationMessage("You Have Failed The Sign In Process");

                return;
            }
        }
    }
}
