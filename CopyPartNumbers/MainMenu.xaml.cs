/* Title:           Main Menu
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
using System.Windows.Shapes;

namespace CopyPartNumbers
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();

        public MainMenu()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnCopyMasterPartNumbers_Click(object sender, RoutedEventArgs e)
        {
            CopyMasterPartList CopyMasterPartList = new CopyMasterPartList();
            CopyMasterPartList.Show();
            Close();
        }

        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            About About = new About();
            About.ShowDialog();
        }

        private void btnCopyPartNumbers_Click(object sender, RoutedEventArgs e)
        {
            CopyPartList CopyPartList = new CopyPartList();
            CopyPartList.Show();
            Close();
        }
    }
}
