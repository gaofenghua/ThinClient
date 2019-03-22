using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.IO;

using System.Windows.Controls.Primitives;
using System.Windows.Input;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors;

//using TC4I_Socket;
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

        public ArrayList Ip { get; set; }
        public string Content { get; set; }
        public string Patten { get; set; }
        public DateTime FromDateTime { get; set; }
        public DateTime ToDateTime { get; set; }

        public List<Customer> Customers { get; set; }

        public byte[] LiveImage { get; set; }


        public SearchViewModel()
        {
            TypeCollection = new ArrayList();
            var strList = Enum.GetNames(typeof(ACAP_TYPE));
            foreach (string key in strList)
            {
                TypeCollection.Add(key);
            }

            Ip = new ArrayList();
            Ip.Add("192.168.77.244");

            Customers = Customer.GetCustomers();

            string base_path = @"E:\BAZZI\GIT\master\LodeStone\src\project\ThinClient\ThinClient";
            LiveImage = TC4I_Common.ReadImageFile(base_path + @".\Sample_Image\FullImage_face.jpg");

            Content = "Hello, how are you";
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
