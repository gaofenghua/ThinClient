using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using socket.framework.Client;
using System.Windows;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using DevExpress.Xpf.Grid;
using ThinClient;

namespace TC4I_Socket
{

    [Serializable]
    struct Socket_Data
    {
        public int Index;
        public string strRev;
        public int iTest;
        public byte[] Photo;
        public byte[] Photo2;
    }
    class TC4I_Socket_Client
    {
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

            string strRev = Encoding.UTF8.GetString(obj);
            MessageBox.Show(strRev);

            Socket_Data socket_Data;
            object deserializedObj = null;
            //deserializeStrToObj(strRev, out deserializedObj);
            deserializeByteToObj(obj, out deserializedObj);
            socket_Data = (Socket_Data)deserializedObj;

            List<Customer> customers = CustomerView.Customers;
            Customer customer = new Customer();
            customer.Name = socket_Data.strRev;
            customer.Photo = socket_Data.Photo;
            customers.Add(customer);

            Task.Factory.StartNew(Begin);

            MessageBox.Show(socket_Data.strRev);
        }

        private void Client_OnConnect(bool obj)
        {
            Console.WriteLine($"pack连接{obj}");
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

        public bool serializeObjToStr(Object obj, out string serializedStr)
        {
            bool serializeOk = false;
            serializedStr = "";
            try
            {
                MemoryStream memoryStream = new MemoryStream();
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, obj);
                serializedStr = System.Convert.ToBase64String(memoryStream.ToArray());

                serializeOk = true;
            }
            catch
            {
                serializeOk = false;
            }

            return serializeOk;
        }

        public bool deserializeStrToObj(string serializedStr, out object deserializedObj)
        {
            bool deserializeOk = false;
            deserializedObj = null;

            try
            {
                byte[] restoredBytes = System.Convert.FromBase64String(serializedStr);
                MemoryStream restoredMemoryStream = new MemoryStream(restoredBytes);
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                deserializedObj = binaryFormatter.Deserialize(restoredMemoryStream);

                deserializeOk = true;
            }
            catch
            {
                deserializeOk = false;
            }

            return deserializeOk;
        }

        public bool serializeObjToByte(Object obj, out byte[] serializedByte)
        {
            bool serializeOk = false;
            serializedByte = null;
            try
            {
                MemoryStream memoryStream = new MemoryStream();
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, obj);
                serializedByte = memoryStream.ToArray();

                serializeOk = true;
            }
            catch
            {
                serializeOk = false;
            }

            return serializeOk;
        }

        public bool deserializeByteToObj(Byte[] serializedByte, out object deserializedObj)
        {
            bool deserializeOk = false;
            deserializedObj = null;

            try
            {
                MemoryStream restoredMemoryStream = new MemoryStream(serializedByte);
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                deserializedObj = binaryFormatter.Deserialize(restoredMemoryStream);

                deserializeOk = true;
            }
            catch
            {
                deserializeOk = false;
            }

            return deserializeOk;
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
            MessageBox.Show("Socket UpdateTb");
            MainWindow mainwin = (MainWindow)Application.Current.MainWindow;
            CardView myTab = mainwin.cardView;
            myTab.MoveLastRow();
        }

    }

    class TC4I_Common
    {
        public static byte[] ReadImageFile(string path)
        {
            FileStream fs = File.OpenRead(path); //OpenRead
            int filelength = 0;
            filelength = (int)fs.Length; //获得文件长度 
            Byte[] image = new Byte[filelength]; //建立一个字节数组 
            fs.Read(image, 0, filelength); //按字节流读取 
            fs.Close();
            return image;
        }
    }
}
