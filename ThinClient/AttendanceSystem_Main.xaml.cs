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

using DevExpress.Xpf.Grid;

namespace ThinClient
{
    /// <summary>
    /// Interaction logic for AttendanceSystem_Main.xaml
    /// </summary>
    public partial class AttendanceSystem_Main : DevExpress.Xpf.Core.ThemedWindow
    {
        public int myInt = 0;
        
        public AttendanceSystem_Main()
        {
            InitializeComponent();
        }
        private void Navigate(string path)
        {
            //string uri = "WPFClient.App.Views." + path;
            string uri = "ThinClient.MainWindow";
            if(myInt%2 == 0)
                this.frmMain.NavigationService.Navigate(new Uri("TestPage.xaml", UriKind.Relative));
            else
                this.frmMain.NavigationService.Navigate(new Uri("SummaryPage.xaml", UriKind.Relative));
  
        }
        private void btnNew_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            //MessageBox.Show("OOO");
            //Navigate("OL");
            //myInt = myInt + 1;
           
            App a = (App)Application.Current;
            for(int i=0;i<10000;i++)
            {
                a.Employee_ViewModel.EmployeeList.Add(new AS_Employee() { Name = "BN CLICK" +i });
            }
            
        }

        private void AS_TreeList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }
        private void DeviceTree_ShowGridMenu(object sender, GridMenuEventArgs e)
        {

        }
 
    }
}
