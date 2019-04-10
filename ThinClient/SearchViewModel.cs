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

namespace ThinClient
{
    public class SearchViewModel
    {
        public enum ACAP_TYPE
        {
            ACAP_ALL = 0,
            ACAP_FACE = 1,
            ACAP_PLATE = 2
        }

        public ArrayList TypeCollection { get; set; }

        private List<UIDevice> _cameraList;
        public List<UIDevice> CameraList { get { return _cameraList; } }
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

            _cameraList = MainWindow.UIDeviceTree.Where(cam => cam.DeviceType == Device_Type.Camera).ToList();

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

            if (Socket_Data_Type.Command_Return == SocketData.DataType)
            {
                Command_Return ret = (Command_Return)SocketData.SubData;
                if (Socket_Command.GetCameraList == ret.Command)
                {
                    List<Camera_Info> cameras = (List<Camera_Info>)ret.Result;
                    var onlineCamList = cameras.Where(cam => cam.Status == 2).ToList();
                    List<UIDevice> uicams = new List<UIDevice>();
                    onlineCamList.ForEach ( it => 
                    {
                        UIDevice uicam = new UIDevice();
                        uicam.ip = it.IP;
                        // other info
                        uicams.Add(uicam);
                    });
                }
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
