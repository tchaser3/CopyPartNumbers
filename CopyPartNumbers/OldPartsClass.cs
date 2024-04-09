/* Title:           Old Parts Class
 * Date:            6-9-17
 * Author:          Terry Holmes */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewEventLogDLL;

namespace CopyPartNumbers
{
    class OldPartsClass
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        //setting up the data
        OldMasterPartsListDataSet aOldMasterPartsListDataSet;
        OldMasterPartsListDataSetTableAdapters.masterpartlistTableAdapter aOldMasterPartsListTableAdapter;

        OldPartNumbersDataSet aOldPartNumbersDataSet;
        OldPartNumbersDataSetTableAdapters.partnumbersTableAdapter aOldPartNumbersTableAdapter;

        public OldPartNumbersDataSet GetOldPartNumbersInfo()
        {
            try
            {
                aOldPartNumbersDataSet = new OldPartNumbersDataSet();
                aOldPartNumbersTableAdapter = new OldPartNumbersDataSetTableAdapters.partnumbersTableAdapter();
                aOldPartNumbersTableAdapter.Fill(aOldPartNumbersDataSet.partnumbers);
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Copy Part Numbers // Old Parts Class // Get Old Part Numbers Info " + Ex.Message);
            }

            return aOldPartNumbersDataSet;
        }
        public OldMasterPartsListDataSet GetOldMasterPartsList()
        {
            try
            {
                aOldMasterPartsListDataSet = new OldMasterPartsListDataSet();
                aOldMasterPartsListTableAdapter = new OldMasterPartsListDataSetTableAdapters.masterpartlistTableAdapter();
                aOldMasterPartsListTableAdapter.Fill(aOldMasterPartsListDataSet.masterpartlist);
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Copy Part Number // Old Parts Class // Get Old Master Parts List " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.Message);
            }

            return aOldMasterPartsListDataSet;
        }
    }
}
