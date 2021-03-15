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

using System.Net;
using socket.framework.Server;
using System.Xml.Linq;
using System.IO;
using TC4I;
using System.Collections.Concurrent;

namespace ThinClient
{
    /// <summary>
    /// Interaction logic for LPR_MainWindow.xaml
    /// </summary>
    public partial class LPR_MainWindow : DevExpress.Xpf.Core.ThemedWindow
    {
        static UdpServer udpServer;

        static ConcurrentQueue<Car_Plate> carQueue = new ConcurrentQueue<Car_Plate>();
        static int nCarIndex = 0;
        static string sPlateNumber = "";
        public LPR_MainWindow()
        {
            InitializeComponent();

            string sDate = DateTime.Now.ToString("yyyy-MM-dd");
            TC4I_Common.m_TxtFile_Name = "CarPlate-" + sDate + ".log";
            TC4I_Common.m_TxtFile_FullName = System.Windows.Forms.Application.StartupPath.ToString() + @"\" + TC4I_Common.m_TxtFile_Name;

            udpServer = new UdpServer(1024);
            udpServer.Start(9005);
            udpServer.OnReceive += UdpServer_OnReceive;

            udpServer.OnSend += UdpServer_OnSend;
            Console.WriteLine("Hello World!");
            Console.Read();
        }

        private void simpleButton_Click(object sender, RoutedEventArgs e)
        {

        }

        public static void UdpServer_OnSend(EndPoint arg1, int arg2)
        {
            Console.WriteLine("发送长度：" + arg2);
        }

        public static void UdpServer_OnReceive(EndPoint arg1, byte[] arg2, int arg3, int arg4)
        {
            Console.WriteLine("接收长度：" + arg4);
            //udpServer.Send(arg1, arg2, arg3, arg4);

            string sData = Encoding.UTF8.GetString(arg2,0,arg4);

            if(true == sData.Contains("plate number"))
            {
                sData = sData.Replace("plate number", "plate_number");
                sData = sData.Replace("plate color", "plate_color");
            }
            else
            {
                return;
            }
            
            sData = "<car>" + sData + "</car>";

            XDocument xd;
            try
            {
                //xd = XDocument.Load(sData);

                StringReader s = new StringReader(sData);
                xd = XDocument.Load(s);
            }
            catch (Exception e)
            {
                //message = e.Message;
                //status = false;
                return;
            }

            var query = from s in xd.Descendants()
                        where s.Name.LocalName == "plate_number" 
                        select s;

            int nQuery = query.Count();
            foreach (XElement item in query)
            {
                if (sPlateNumber != item.Value)
                {
                    sPlateNumber = item.Value;
                }
                else
                {
                    continue;
                }
                //string sPlateNumber = item.Value;


                TC4I_Common.WriteTxtFile(sPlateNumber);

                Car_Plate car = new Car_Plate();
                car.Coming_Time = DateTime.Now;
                car.sNumber = sPlateNumber;
                car.nIndex = nCarIndex;
                nCarIndex = nCarIndex + 1;

                carQueue.Enqueue(car);     
            }

        }
    }

    public class Car_Plate
    {
        public int nIndex;
        public string sNumber;
        public DateTime Coming_Time;
    }
}
