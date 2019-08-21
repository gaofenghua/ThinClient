using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TC4I;
using System.ComponentModel;
using System.Windows.Data;

namespace ThinClient
{
    class DataSource
    {
    }

    public class Customer
    {
        public string Name { get; set; }
        public string City { get; set; }
        public int Visits { get; set; }
        public DateTime? Birthday { get; set; }

        public byte[] Photo { get; set; }
        public byte[] Photo2 { get; set; }


        public static List<Customer> GetCustomers()
        {
            List<Customer> people = new List<Customer>();

            /*
            Customer newCustomer = new Customer() { Name = "车牌：沪A-123456", City = "摄像机：长寿路万航渡路", Visits = 19, Birthday = new DateTime(1971, 10, 3) };
            //newCustomer.Photo = newCustomer.GetPhoto();
            newCustomer.Photo = TC4I_Common.ReadImageFile("d:\\Axis_Code\\Sample_Image\\car\\sample_plate.png");
            newCustomer.Photo2 = TC4I_Common.ReadImageFile("d:\\Axis_Code\\Sample_Image\\car\\下载.jpg");
            people.Add(newCustomer);

            Customer newCustomer2 = new Customer() { Name = "车牌：京A-888888", City = "摄像机：长寿路万航渡路", Visits = 19, Birthday = new DateTime(1972, 10, 3) };
            //newCustomer.Name = "车牌：京A-123456";
            //newCustomer.City = "摄像机：长寿路万航渡路";
            //newCustomer.Birthday = new DateTime(1972, 10, 3);
            newCustomer2.Photo = TC4I_Common.ReadImageFile("d:\\Axis_Code\\Sample_Image\\car\\sample_plate.jpg");
            newCustomer2.Photo2 = TC4I_Common.ReadImageFile("d:\\Axis_Code\\Sample_Image\\car\\下载 (1).jpg");
            people.Add(newCustomer2);

            Customer newCustomer3 = new Customer() { Name = "车牌：京A-888888", City = "摄像机：长寿路万航渡路", Visits = 19, Birthday = new DateTime(1972, 10, 3) };
            newCustomer3.Photo = TC4I_Common.ReadImageFile("d:\\Axis_Code\\Sample_Image\\car\\sample_plate2.png");
            newCustomer3.Photo2 = TC4I_Common.ReadImageFile("d:\\Axis_Code\\Sample_Image\\car\\下载 (1).jpg");
            people.Add(newCustomer3);

            Customer newCustomer4 = new Customer() { Name = "车牌：京A-888888", City = "摄像机：长寿路万航渡路", Visits = 19, Birthday = new DateTime(1972, 10, 3) };
            newCustomer4.Photo = TC4I_Common.ReadImageFile("d:\\Axis_Code\\Sample_Image\\car\\sample_plate3.png");
            newCustomer4.Photo2 = TC4I_Common.ReadImageFile("d:\\Axis_Code\\Sample_Image\\car\\下载 (1).jpg");
            people.Add(newCustomer4);

            Customer newCustomer5 = new Customer() { Name = "车牌：京A-888888", City = "摄像机：长寿路万航渡路", Visits = 19, Birthday = new DateTime(1972, 10, 3) };
            newCustomer5.Photo = TC4I_Common.ReadImageFile("d:\\Axis_Code\\Sample_Image\\car\\sample_plate4.png");
            newCustomer5.Photo2 = TC4I_Common.ReadImageFile("d:\\Axis_Code\\Sample_Image\\car\\下载 (1).jpg");
            people.Add(newCustomer5);

            Customer newCustomer6 = new Customer() { Name = "车牌：京A-888888", City = "摄像机：长寿路万航渡路", Visits = 19, Birthday = new DateTime(1972, 10, 3) };
            newCustomer6.Photo = TC4I_Common.ReadImageFile("d:\\Axis_Code\\Sample_Image\\car\\sample_plate5.png");
            newCustomer6.Photo2 = TC4I_Common.ReadImageFile("d:\\Axis_Code\\Sample_Image\\car\\下载 (1).jpg");
            people.Add(newCustomer6);
            */

            Customer newCustomer = new Customer() { Name = "姓名：无识别模块", City = "摄像机：办公室1", Visits = 19, Birthday = new DateTime(1971, 10, 3) };
            newCustomer.Photo = TC4I_Common.ReadImageFile("d:\\Axis_Code\\Sample_Image\\face\\20181210144646-0.jpg");//20181210144646-0.jpg
            newCustomer.Photo2 = TC4I_Common.ReadImageFile("d:\\Axis_Code\\Sample_Image\\face\\sample_full.png");
            people.Add(newCustomer);

            Customer newCustomer2 = new Customer() { Name = "姓名：无识别模块", City = "摄像机：办公室1", Visits = 19, Birthday = new DateTime(1972, 10, 3) };
            newCustomer2.Photo = TC4I_Common.ReadImageFile("d:\\Axis_Code\\Sample_Image\\face\\20181210150705-0.jpg");
            newCustomer2.Photo2 = TC4I_Common.ReadImageFile("d:\\Axis_Code\\Sample_Image\\face\\sample_full.png");
            people.Add(newCustomer2);

            Customer newCustomer3 = new Customer() { Name = "车牌：京A-888888", City = "摄像机：长寿路万航渡路", Visits = 19, Birthday = new DateTime(1972, 10, 3) };
            newCustomer3.Photo = TC4I_Common.ReadImageFile("d:\\Axis_Code\\Sample_Image\\face\\20181210150920-0.jpg");
            newCustomer3.Photo2 = TC4I_Common.ReadImageFile("d:\\Axis_Code\\Sample_Image\\face\\sample_full.png");
            people.Add(newCustomer3);

            Customer newCustomer4 = new Customer() { Name = "车牌：京A-888888", City = "摄像机：长寿路万航渡路", Visits = 19, Birthday = new DateTime(1972, 10, 3) };
            newCustomer4.Photo = TC4I_Common.ReadImageFile("d:\\Axis_Code\\Sample_Image\\face\\20181210152248-0.jpg");
            newCustomer4.Photo2 = TC4I_Common.ReadImageFile("d:\\Axis_Code\\Sample_Image\\car\\下载 (1).jpg");
            people.Add(newCustomer4);

            Customer newCustomer5 = new Customer() { Name = "车牌：京A-888888", City = "摄像机：长寿路万航渡路", Visits = 19, Birthday = new DateTime(1972, 10, 3) };
            newCustomer5.Photo = TC4I_Common.ReadImageFile("d:\\Axis_Code\\Sample_Image\\face\\20181211133729-0.jpg");
            newCustomer5.Photo2 = TC4I_Common.ReadImageFile("d:\\Axis_Code\\Sample_Image\\car\\下载 (1).jpg");
            people.Add(newCustomer5);

            Customer newCustomer6 = new Customer() { Name = "车牌：京A-888888", City = "摄像机：长寿路万航渡路", Visits = 19, Birthday = new DateTime(1972, 10, 3) };
            newCustomer6.Photo = TC4I_Common.ReadImageFile("d:\\Axis_Code\\Sample_Image\\face\\20181211100005-0.jpg");
            newCustomer6.Photo2 = TC4I_Common.ReadImageFile("d:\\Axis_Code\\Sample_Image\\car\\下载 (1).jpg");
            people.Add(newCustomer6);


            people.Add(new Customer() { Name = "Gregory S. Price", City = "Huntington", Visits = 4, Birthday = new DateTime(1980, 1, 1) });
            people.Add(new Customer() { Name = "Irma R. Marshall", City = "Hong Kong", Visits = 2, Birthday = new DateTime(1966, 4, 15) });
            people.Add(new Customer() { Name = "John C. Powell", City = "Luogosano", Visits = 6, Birthday = new DateTime(1982, 3, 11) });
            people.Add(new Customer() { Name = "Christian P. Laclair", City = "Clifton", Visits = 11, Birthday = new DateTime(1977, 12, 5) });
            people.Add(new Customer() { Name = "Karen J. Kelly", City = "Madrid", Visits = 8, Birthday = new DateTime(1956, 9, 5) });
            people.Add(new Customer() { Name = "Brian C. Cowling", City = "Los Angeles", Visits = 5, Birthday = new DateTime(1990, 2, 27) });
            people.Add(new Customer() { Name = "Thomas C. Dawson", City = "Rio de Janeiro", Visits = 21, Birthday = new DateTime(1965, 5, 5) });
            people.Add(new Customer() { Name = "Angel M. Wilson", City = "Selent", Visits = 8, Birthday = new DateTime(1987, 11, 9) });
            people.Add(new Customer() { Name = "Winston C. Smith", City = "London", Visits = 1, Birthday = new DateTime(1949, 6, 18) });
            people.Add(new Customer() { Name = "Harold S. Brandes", City = "Bangkok", Visits = 3, Birthday = new DateTime(1989, 1, 8) });
            people.Add(new Customer() { Name = "Michael S. Blevins", City = "Harmstorf", Visits = 4, Birthday = new DateTime(1972, 9, 14) });
            people.Add(new Customer() { Name = "Jan K. Sisk", City = "Naples", Visits = 6, Birthday = new DateTime(1989, 5, 7) });
            people.Add(new Customer() { Name = "Sidney L. Holder", City = "Watauga", Visits = 19, Birthday = new DateTime(1971, 10, 3) });

  

            //foreach(Customer customer in people)
            //{
            //    customer.Photo = customer.GetPhoto();
            //}
            
            return people;
        }

        byte[] GetPhoto()
        {
            //string name = string.Format("{0}{1}.jpg", contact.FirstName, contact.LastName);
            string name = string.Format("EdwardKeck.jpg");
            string path = "pack://application:,,,/Photos/" + name;
            Stream resourceStream = App.GetResourceStream(new Uri(path)).Stream;
            byte[] res;
            using (BinaryReader reader = new BinaryReader(resourceStream))
            {
                res = reader.ReadBytes((int)resourceStream.Length);
            }
            return res;
        }
    }

    public class CustomerView
    {
        public CustomerView()
        {
            Customers = Customer.GetCustomers();
        }
        public static List<Customer> Customers { get; set; }
    }

    public class SmallImage
    {
        public byte[] S_Photo { get; set; }
    }

    public class AS_Employee
    {
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Department { get; set; }
        public DateTime? PunchIn { get; set; }
        public DateTime? PunchOut { get; set; }
        public string Status { get; set; }
    }

    public class AS_Employee_View
    {
        public static List<AS_Employee> EmployeeList { get; set; }
        public AS_Employee_View()
        {
            EmployeeList = new List<AS_Employee>();
        }
    }
}
