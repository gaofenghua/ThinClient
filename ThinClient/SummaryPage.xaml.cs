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

namespace ThinClient
{
    /// <summary>
    /// Interaction logic for SummaryPage.xaml
    /// </summary>
    public partial class SummaryPage : Page
    {
        public SummaryPage()
        {
            AS_Employee_View b = new AS_Employee_View();
         
            InitializeComponent();

            AS_Employee_View.EmployeeList.Add(new AS_Employee() { Name = "Gregory S. Price" });
            my.ItemsSource = AS_Employee_View.EmployeeList;
           
        }
    }
}
