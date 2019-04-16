using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.IO;
using System.Configuration;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors;
using TC4I;
using TransactionServer;
using DevExpress.Mvvm;
using System.Diagnostics;

namespace ThinClient
{
    public class SearchViewModel : BindableBase
    {
        public enum ACAP_TYPE
        {
            ACAP_ALL = 0,
            ACAP_FACE = 1,
            ACAP_PLATE = 2
        }

        public ArrayList TypeCollection { get; set; }

        private List<UIDevice> _cameraList;
        public List<UIDevice> CameraList
        {
            get { return _cameraList; }
            set { SetProperty(ref _cameraList, value, "CameraList"); }
        }
        public UIDevice CurrentCamera { get; set; }

        public string Content { get; set; }
        public string Patten { get; set; }
        public DateTime FromDateTime { get; set; }
        public DateTime ToDateTime { get; set; }

        public List<Customer> Customers { get; set; }

        public byte[] LiveImage { get; set; }

        private TC4I_Socket_Client SocketClient = null;
        private string ServerIp = string.Empty;
        private int ServerPort = 0;


        //private UIDevice 

        public SearchViewModel()
        {
            TypeCollection = new ArrayList();
            var strList = Enum.GetNames(typeof(ACAP_TYPE));
            foreach (string key in strList)
            {
                TypeCollection.Add(key);
            }

            CameraList = MainWindow.UIDeviceTree.Where(cam => cam.DeviceType == Device_Type.Camera).ToList();

            Customers = Customer.GetCustomers();

            string base_path = @"E:\BAZZI\GIT\master\LodeStone\src\project\ThinClient\ThinClient";
            LiveImage = TC4I_Common.ReadImageFile(base_path + @".\Sample_Image\FullImage_face.jpg");

            Content = "Hello, how are you";

            ServerIp = ConfigurationManager.AppSettings["ServerIP"];
            ServerPort = Int32.Parse(ConfigurationManager.AppSettings["ServerPort"]);
            SocketClient = new TC4I_Socket_Client(1024, ServerIp, ServerPort, 0xFF);
            SocketClient.DataEvent += OnReceiveData;
            SocketClient.Connect();
        }


        private void OnReceiveData(Object obj)
        {
            Socket_Data SocketData = (Socket_Data)obj;

            //List<UIDevice> uicams = new List<UIDevice>();
            if (Socket_Data_Type.Command == SocketData.DataType)
            {
                List<UIDevice> uicams = new List<UIDevice>();
                Command_Request ret = (Command_Request)SocketData.SubData;
                if (Socket_Command.UpdateCameraList == ret.Command)
                {
                    Camera_Info[] cameras = (Camera_Info[])ret.Arg;
                    var onlineCamList = cameras.Where(cam => cam.Status == DEVICE_STATE.DEVICE_ONLINE).ToList();
                    
                    onlineCamList.ForEach(it =>
                  {
                      UIDevice uicam = new UIDevice();
                      uicam.ip = it.IP;
                      // other info
                      uicams.Add(uicam);
                  });
                }
                this.CameraList = uicams;
            }
        }
    }


    public class HorizontalScrollingOnMouseWheelBehavior : Behavior<ListBoxEdit>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewMouseWheel += OnPreviewMouseWheel;
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewMouseWheel -= OnPreviewMouseWheel;
        }
        void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scrollBar = (ScrollBar)LayoutHelper.FindElementByName(AssociatedObject, "PART_VerticalScrollBar");
            if (e.Delta > 0)
                ScrollBar.LineUpCommand.Execute(null, scrollBar);
            else if (e.Delta < 0)
            {
                ScrollBar.LineDownCommand.Execute(null, scrollBar);
            }
        }

    }

}
