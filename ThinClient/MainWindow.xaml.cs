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
using System.Threading;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Data;
using System.ComponentModel;
using TC4I;
using System.Configuration;
using DevExpress.Xpf.Grid.TreeList;
using DevExpress.Xpf.Bars;

namespace ThinClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : DevExpress.Xpf.Core.ThemedWindow
    {
        public static List<UIDevice> UIDeviceTree { get; set; }

        private TC4I_Socket_Client CC_Client = null;
        public MainWindow()
        {
            InitializeComponent();

            //cardView.CardHeaderTemplate = (DataTemplate)this.Resources["cardHeaderTemplate"];
            cardView2.CardHeaderTemplate = (DataTemplate)this.Resources["cardHeaderTemplate"];

            string ServerIP = null;
            int ServerPort = -1;
            string ServerName = null;
            try
            {
                ServerIP = ConfigurationManager.AppSettings["ServerIP"];
                ServerPort = Int32.Parse(ConfigurationManager.AppSettings["ServerPort"]);
            }
            catch(Exception e)
            {
                String message = "配置文件：ServerIP，ServerPort\r\n" + e.Message;
                MessageBox.Show(message);
                return;
            }

            ServerName = ConfigurationManager.AppSettings["ServerName"];
            if(ServerName == null)
            {
                ServerName = ServerIP;
            }

            UIDeviceTree = new List<UIDevice>();
            UIDevice node = new UIDevice();
            node.ID = 0;
            node.ParentID = 0;
            node.name = ServerName;
            node.ip = ServerIP;
            node.status = "离线1";
            node.DeviceType = Device_Type.Server;
            UIDeviceTree.Add(node);

            UIDevice node2 = new UIDevice();
            node2.ID = 2;
            node2.ParentID = 0;
            node2.name = "ACAP camera 01";
            node2.ip = "192.168.1.2";
            node2.DeviceType = Device_Type.Camera;
            UIDeviceTree.Add(node2);

            myTreeListControl.ItemsSource = UIDeviceTree;
        
            CC_Client = new TC4I_Socket_Client(1024,ServerIP,ServerPort,0xFF);
            CC_Client.DataEvent += OnSocketDataEvent;
            CC_Client.Connect();
        }

        private readonly TaskScheduler _syncContextTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

        private void Button_Click()
        {
            Task.Factory.StartNew(SchedulerWork);
        }

        private void SchedulerWork()
        {
            Task.Factory.StartNew(Begin).Wait();

        }

        private void Begin()
        {
            Task.Factory.StartNew(() => UpdateTb(),
                    new CancellationTokenSource().Token, TaskCreationOptions.None, _syncContextTaskScheduler).Wait();
        }
        private void UpdateTb()
        {
            MessageBox.Show("OK");

            List<Customer> myCus = CustomerView.Customers;
            Customer newCustomer = new Customer() { Name = "Thread", City = "Add", Visits = 19, Birthday = new DateTime(1971, 10, 3) };
            //newCustomer.Photo = newCustomer.GetPhoto();
            myCus.Add(newCustomer);
            myCus.Add(newCustomer);
            cardView.MoveLastRow();
            cardView2.MoveLastRow();
        }
        private void DeviceTree_ShowGridMenu(object sender, GridMenuEventArgs e)
        {
            var le = e.TargetElement as LightweightCellEditor;
            if (le == null)
                return;
            var d = le.DataContext as TreeListRowData;
            var item = d.Row as UIDevice;
            var st = item.name;
            var b = new BarButtonItem();
            b.Content = st;
            b.ItemClick += btnNew_ItemClick;

            e.Customizations.Add(b);
        }
     
        private void btnNew_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            if (!CC_Client.RemoteCommand_GetCameraList())
            {
                MessageBox.Show("GetCameraList failed, Please check the server connection.");
            }
        }
        private void treeListTagsControl_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TreeListViewHitInfo info = myTreeListControl.View.CalcHitInfo(e.OriginalSource as DependencyObject);
            if (!info.InRow)
                return;
            MessageBox.Show("ID: " + ((UIDevice)myTreeListControl.View.GetNodeByRowHandle(info.RowHandle).Content).ID);
        }
        public void OnSocketDataEvent(object Arg)
        {
            Socket_Data SocketData = (Socket_Data)Arg;
            switch (SocketData.DataType)
            {
                case Socket_Data_Type.Server_Status:
                    foreach (UIDevice node in UIDeviceTree)
                    {
                        if(node.DeviceType == Device_Type.Server)
                        {
                            Socket_Status ServerStatus = (Socket_Status)SocketData.SubData;
                            if(ServerStatus == Socket_Status.Normal)
                            {
                                node.status = "连线";
                            }
                            else
                            {
                                node.status = "离线";
                            }
                            
                        }
                    }
                    break;
                case Socket_Data_Type.Command_Return:
                    OnRemoteCommandReturn(SocketData.SubData);
                    break;
            }
            Task.Factory.StartNew(() => UpdateDeviceTree(),
                    new CancellationTokenSource().Token, TaskCreationOptions.None, _syncContextTaskScheduler);
        }
        public void OnRemoteCommandReturn(object Arg)
        {
            MessageBox.Show("Command return");
            Command_Return CommandReturn = (Command_Return) Arg;
            switch(CommandReturn.Command)
            {
                case Socket_Command.GetCameraList:
                    Camera_Info[] CameraList = (Camera_Info[]) CommandReturn.Result;
                    UIDeviceTree_Clear();
                    foreach (Camera_Info camera in CameraList)
                    {
                        UIDevice node = new UIDevice();
                        node.name = camera.Name;
                        node.ip = camera.IP;
                        node.DeviceType = Device_Type.Camera;
                        node.status = "Online";
                        UIDeviceTree_Add(node);
                    }
                    break;
            }

            Task.Factory.StartNew(() => UpdateDeviceTree(),
                    new CancellationTokenSource().Token, TaskCreationOptions.None, _syncContextTaskScheduler);
        }
        private void UpdateDeviceTree()
        {
            myTreeListControl.RefreshData();
        }
        public void UIDeviceTree_Clear()
        {
            for(int i=UIDeviceTree.Count()-1;i>-1;i--)
            {
                if(UIDeviceTree[i].DeviceType == Device_Type.Camera)
                {
                    UIDeviceTree.RemoveAt(i);
                }
            }
        }
        public void UIDeviceTree_Add(UIDevice node)
        {
            int ParentNodeID = UIDeviceTree_GetParentID();
            if(ParentNodeID== -1)
            {
                return;
            }

            node.ParentID = ParentNodeID;
            node.ID = UIDeviceTree.Count();
            UIDeviceTree.Add(node);
        }

        public int UIDeviceTree_GetParentID()
        {
            foreach (UIDevice node in UIDeviceTree)
            {
                if (node.DeviceType == Device_Type.Server)
                {
                    return node.ID;
                }
            }
            return -1;
        }
    }

    public class CustomerToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Customer customer = value as Customer;
            if (customer == null)
                return null;

            /*
            if (customer.Visits % 3 == 0)
                return new SolidColorBrush(Colors.Violet);
            
            return new SolidColorBrush(Colors.Green);
            */
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CustomerToHeaderTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Customer customer = value as Customer;
            if (customer == null)
                return null;

            //return customer.Name;
            return customer.Birthday;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class CardRowTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Template1 { get; set; }
        public DataTemplate Template2 { get; set; }
        public DataTemplate Template3 { get; set; }
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            EditGridCellData d = (EditGridCellData)item;

            //if (d.Column.FieldName == "Name")
            if (d.Column.FieldName == "Name" || d.Column.FieldName == "City" || d.Column.FieldName == "Visits" || d.Column.FieldName == "Birthday")
                return Template3;
            else return Template1;

            //Column column = (Column)item;
            //return (DataTemplate)((Control)container).FindResource(column.Settings + "ColumnTemplate");
        }
    }

    public class CardRowTemplateSelector_Bottom : DataTemplateSelector
    {
        public DataTemplate Template1 { get; set; }
        public DataTemplate Template2 { get; set; }
        public DataTemplate Template3 { get; set; }
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            EditGridCellData d = (EditGridCellData)item;

            //if (d.Column.FieldName == "Name")
            if (d.Column.FieldName == "Name" || d.Column.FieldName == "City" || d.Column.FieldName == "Visits" || d.Column.FieldName == "Birthday")
                return Template3;
            else return Template2;

            //Column column = (Column)item;
            //return (DataTemplate)((Control)container).FindResource(column.Settings + "ColumnTemplate");
        }
    }

    public class UIDeviceTreeListNodeImageSelector : TreeListNodeImageSelector
    {
        public override ImageSource Select(TreeListRowData rowData)
        {
            if (rowData.Row == null) return null;
            UIDevice node = rowData.Row as UIDevice;

            string path = "pack://application:,,,/Icon/server.png";
            if (node.DeviceType == Device_Type.Camera)
            {
                path = "pack://application:,,,/Icon/camera.png";
            }
            BitmapImage image = new BitmapImage(new Uri(path));
            //FormatConvertedBitmap grayBitmapSource = new FormatConvertedBitmap();
            //grayBitmapSource.BeginInit();
            //grayBitmapSource.Source = image;
            //if (false)
            //{
            //    grayBitmapSource.DestinationFormat = PixelFormats.Gray32Float;
            //}
            //grayBitmapSource.EndInit();
            //return grayBitmapSource;
            return image;
        }
    }

    public enum Device_Type { Server, Camera};
    public class UIDevice
    {
        public int ID { get; set; }
        public int ParentID { get; set; }
        public string ip { set; get; }
        public string status { set; get; }
        public string name;
        public Device_Type DeviceType;
        public int DeviceID;
        public UIDevice()
        {

        }
    }
}
