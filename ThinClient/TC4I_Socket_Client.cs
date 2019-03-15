using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using socket.framework.Client;
using System.Windows;
using DevExpress.Xpf.Grid;
using ThinClient;
using TC4I;

namespace TC4I
{
    class TC4I_Socket_Client
    {
        public event Action<object> OnRemoteCommandReturn;

        public Socket_Status status;
        System.Threading.Timer heartbeat_timer = null;
        int Time_Interval = 3000;
        int heartbeat = 0;
        public byte[] heartbeat_package = null;

        public int id = -1;
        public string server_ip=null;
        public int server_port = -1;

        TcpPackClient client;
        public TC4I_Socket_Client(int receiveBufferSize, string ip, int port, uint headerFlag)
        {
            client = new TcpPackClient(receiveBufferSize, headerFlag);
            client.OnConnect += Client_OnConnect;
            client.OnReceive += Client_OnReceive;
            client.OnSend += Client_OnSend;
            client.OnClose += Client_OnClose;
            client.OnDisconnect += Client_OnDisconnect;

            client.Connect(ip, port);

            server_ip = ip;
            server_port = port;

            // Initial heartbeat package
            Socket_Data SocketData = new Socket_Data();
            SocketData.DataType = Socket_Data_Type.Heartbeat;
            Heartbeat_Data HeartbeatData = new Heartbeat_Data();
            HeartbeatData.ClientInfo = "My Client";
            SocketData.SubData = HeartbeatData;
            TC4I_Socket.serializeObjToByte(SocketData, out heartbeat_package);

        }

        private void Client_OnClose()
        {
            Console.WriteLine($"pack断开");
        }
        private void Client_OnDisconnect()
        {
            Console.WriteLine($"pack中断");
        }

        private void Client_OnReceive(byte[] obj)
        {
            Console.WriteLine($"pack接收byte[{obj.Length}]");

            Parse_Receive_Data(obj);
        }
        public void Parse_Receive_Data(byte[] rev)
        {
            Socket_Data revData;
            object deserializedObj = null;
            TC4I_Socket.deserializeByteToObj(rev, out deserializedObj);
            revData = (Socket_Data)deserializedObj;

            switch (revData.DataType)
            {
                case Socket_Data_Type.Heartbeat:
                    heartbeat = 0;
                    break;
                case Socket_Data_Type.Camera_Data:
                    Camera_Data CameraData = (Camera_Data)revData.SubData;
                    List<Customer> customers = CustomerView.Customers;
                    Customer customer = new Customer();
                    customer.Name = CameraData.strInfo;
                    customer.Photo = CameraData.Photo;
                    customers.Add(customer);

                    Task.Factory.StartNew(Begin);

                    //MessageBox.Show(revData.strInfo);
                    break;
                case Socket_Data_Type.Command_Return:
                    Client_OnRemoteCommandReturn((Command_Return)revData.SubData);
                   // Task.Factory.StartNew(Begin);
                    break;
            }

        }

        public void Client_OnRemoteCommandReturn(Command_Return CommandReturn)
        {
            switch(CommandReturn.Command)
            {
                case Socket_Command.GetCameraList:
                    Camera_Info[] CameraList = (Camera_Info[])CommandReturn.Result;
                    if(OnRemoteCommandReturn != null)
                    {
                        OnRemoteCommandReturn(CommandReturn);
                    }
                    break;
            }
        }
        private void Client_OnConnect(bool obj)
        {
            if (obj == false)
            {
                status = Socket_Status.Connect_Failed;

                TC4I_Common.PrintLog(0, String.Format("error: Connect failed, server={0}:{1}",server_ip,server_port));
            }
            else
            {
                status = Socket_Status.Normal;

                TC4I_Common.PrintLog(0, String.Format("Server Connected, server={0}:{1}",server_ip,server_port));
            }

            if (heartbeat_timer == null)
            {
                heartbeat_timer = new Timer(HeartBeat, null, Time_Interval, Timeout.Infinite);
            }
            else
            {
                heartbeat_timer.Change(Time_Interval, Timeout.Infinite);
            }
        }

        public void Send(byte[] data, int offset, int length)
        {
            client.Send(data, offset, length);

        }

        private void Client_OnSend(int obj)
        {
            Console.WriteLine($"pack已发送长度{obj}");
        }

        public void Close()
        {
            client.Close();
        }

        public void ReConnect()
        {

        }
        public void HeartBeat(object obj)
        {
            if (heartbeat < -5)
            {
                if (status != Socket_Status.Connecting)
                {
                    TC4I_Common.PrintLog(0, String.Format("Warning: Socket Heartbeat failed. Start Reconnect, client.connected = {0}, id={1}.", client.Connected, id));
                    ReConnect();
                    heartbeat = 0;
                    return;
                }
            }

            if (client.Connected == true)
            {
                Send(heartbeat_package, 0, heartbeat_package.Length);

                if (id == 20)
                {
                    TC4I_Common.PrintLog(2, String.Format("Debugging: HeartBeat sent, client.connected = {0}, status={2}, id={1}.", client.Connected, id, status));
                }
            }
            heartbeat = heartbeat - 1;
            TC4I_Common.PrintLog(0, String.Format("Heartbeat:"));
            heartbeat_timer.Change(Time_Interval, Timeout.Infinite);
        }
 
        public bool RemoteCommand_GetCameraList()
        {
            Command_Request CommandRequest = new Command_Request();
            CommandRequest.Command = Socket_Command.GetCameraList;
            CommandRequest.Arg = null;

            Socket_Data SocketData = new Socket_Data();
            SocketData.DataType = Socket_Data_Type.Command;
            SocketData.SubData = CommandRequest;

            byte[] SocketPackage = null;
            TC4I_Socket.serializeObjToByte(SocketData, out SocketPackage);
            Send(SocketPackage, 0, SocketPackage.Length);

            return true;
        }

        private readonly TaskScheduler _syncContextTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        private void SchedulerWork()
        {
            Task.Factory.StartNew(Begin).Wait();

        }
        private void Begin()
        {
            Task.Factory.StartNew(() => UpdateTb(),
                    new CancellationTokenSource().Token, TaskCreationOptions.None, _syncContextTaskScheduler);

        }
        private void UpdateTb()
        {
            //MessageBox.Show("Socket UpdateTb");
            MainWindow mainwin = (MainWindow)Application.Current.MainWindow;
            CardView myTab = mainwin.cardView;
            myTab.MoveLastRow();
            mainwin.myTreeListControl.ItemsSource = null;
        }
    }
}
