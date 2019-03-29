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
    public class TC4I_Socket_Client
    {
        public event Action<object> OnRemoteCommandReturn;
        public event Action<object> DataEvent;

        public Socket_Status status;
        System.Threading.Timer heartbeat_timer = null;
        int Time_Interval = 3000;
        int heartbeat = 0;
        bool Stop_Heartbeat_Timer = false;
        public byte[] heartbeat_package = null;

        DateTime First_Reconnect_time = DateTime.MinValue;
        int Max_Reconnect_times = 5;
        TimeSpan Min_TimeSpan = TimeSpan.FromMinutes(5);
        int Reconnect_Times = 0;
        int Connect_Success_Times = 0;

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

            status = Socket_Status.Init;

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
        public void Connect()
        {
            if(client != null)
            {
                client.Connect(server_ip, server_port);
                status = Socket_Status.Connecting;
            }
        }
        private void Client_OnClose()
        {
            Console.WriteLine($"pack断开");
        }
        private void Client_OnDisconnect()
        {
            Console.WriteLine($"pack中断");

            status = Socket_Status.Connect_Failed;
            FireEvent_ServerStatusChange();
        }

        private void Client_OnReceive(byte[] obj)
        {
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
                default :
                    if (DataEvent != null)
                    {
                        DataEvent(revData);
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
                Connect_Success_Times += 1;
                TC4I_Common.PrintLog(0, String.Format("Server Connected, server={0}:{1}",server_ip,server_port));
            }

            FireEvent_ServerStatusChange();

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
            Stop_Heartbeat_Timer = true;
            
            client.Close();
            status = Socket_Status.Closed;

            FireEvent_ServerStatusChange();
        }
        public bool Check_Reconnect_TooMany()
        {
            if (First_Reconnect_time == DateTime.MinValue)
            {
                First_Reconnect_time = DateTime.Now;
            }

            if (Reconnect_Times > Max_Reconnect_times)
            {
                TimeSpan timespan = DateTime.Now - First_Reconnect_time;
                if (timespan < Min_TimeSpan || Reconnect_Times > Max_Reconnect_times * 3)
                {
                    return true;
                }
            }
            return false;
        }
        public void ReConnect()
        {
            if (status == Socket_Status.Connecting)
            {
                return;
            }

            if (Check_Reconnect_TooMany() == true)
            {
                TC4I_Common.PrintLog(0, String.Format("error: Socket reconnect too frequently, Force close. id={0}. {1} - {2} reconnect {3} times", id, First_Reconnect_time, DateTime.Now, Reconnect_Times));

                Close();
                status = Socket_Status.Closed;
                return;
            }

            Reconnect_Times = Reconnect_Times + 1;
            status = Socket_Status.Connecting;

            TC4I_Common.PrintLog(0, String.Format("Warning: Socket ReConnecting, client.connected = {0}, id={1}.", client.Connected, id));

            client.Close();
            Thread.Sleep(500);
            if(Connect_Success_Times == 0)
            {
                client.Connect(server_ip, server_port);
            }
            else
            {
                client.Reconnect(server_ip, server_port);
            }
        }
        public void HeartBeat(object obj)
        {
            if(Stop_Heartbeat_Timer == true)
            {
                return;
            }

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
                
                TC4I_Common.PrintLog(2, String.Format("Debugging: HeartBeat sent, client.connected = {0}, status={2}, id={1}.", client.Connected, id, status));
            }
            heartbeat = heartbeat - 1;
            heartbeat_timer.Change(Time_Interval, Timeout.Infinite);
        }
 
        public bool RemoteCommand_GetCameraList()
        {
            if(status != Socket_Status.Normal)
            {
                return false;
            }

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

        public void FireEvent_ServerStatusChange()
        {
            Socket_Data SocketData = new Socket_Data();
            SocketData.DataType = Socket_Data_Type.Server_Status;
            SocketData.SubData = status;
            if (DataEvent != null)
            {
                DataEvent(SocketData);
            }
        }
        public void Client_OnRemoteCommandReturn(Command_Return CommandReturn)
        {
            switch (CommandReturn.Command)
            {
                case Socket_Command.GetCameraList:
                    Camera_Info[] CameraList = (Camera_Info[])CommandReturn.Result;
                    if (DataEvent != null)
                    {
                        DataEvent(CommandReturn);
                    }
                    break;
            }
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
