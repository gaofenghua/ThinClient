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
            //this.frmMain.NavigationService.Navigate(new Button());
            Type type = Type.GetType(uri);
            //if (type != null)
            //{
            //    //实例化Page页
            //    object obj = type.Assembly.CreateInstance(uri);
            //    UserControl control = obj as UserControl;
            //    this.frmMain.Content = control;

            //    MainWindow myView = obj as MainWindow;
            //    //myView.ParentWindow = this;
            //   // myView.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            //    //myView.Owner = this.frmMain;
            //    //myView.ShowDialog();

            //    //PropertyInfo[] infos = type.GetProperties();
            //    //foreach (PropertyInfo info in infos)
            //    //{
            //    //    //将MainWindow设为page页的ParentWin
            //    //    if (info.Name == "ParentWindow")
            //    //    {
            //    //        info.SetValue(control, this, null);
            //    //        break;
            //    //    }
            //    //}
            //}
        }
        private void btnNew_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            //MessageBox.Show("OOO");
            Navigate("OL");
            myInt = myInt + 1;
        }

        private void AS_TreeList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }
        private void DeviceTree_ShowGridMenu(object sender, GridMenuEventArgs e)
        {

        }
    }
}
