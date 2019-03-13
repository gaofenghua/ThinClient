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

            Socket_Data heartbeat_data = new Socket_Data();
            heartbeat_data.Data_Type = (int)Socket_Data_Type.Heartbeat;
            heartbeat_data.strInfo = "My client name";

            TC4I_Socket.serializeObjToByte(heartbeat_data, out heartbeat_package);
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

            switch (revData.Data_Type)
            {
                case Socket_Data_Type.Heartbeat:
                    heartbeat = 0;
                    break;
                case Socket_Data_Type.Camera_Data:
                    List<Customer> customers = CustomerView.Customers;
                    Customer customer = new Customer();
                    customer.Name = revData.strInfo;
                    customer.Photo = revData.Photo;
                    customers.Add(customer);

                    Task.Factory.StartNew(Begin);

                    //MessageBox.Show(revData.strInfo);
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

        }
    }
}
