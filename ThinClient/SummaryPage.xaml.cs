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
using System.ComponentModel;

namespace ThinClient
{
    /// <summary>
    /// Interaction logic for SummaryPage.xaml
    /// </summary>
    public partial class SummaryPage : Page
    {
       
        public SummaryPage()
        {
            
            InitializeComponent();

            App a = (App)Application.Current;

            GridControl_Summary.ItemsSource = a.Employee_ViewModel.EmployeeList;

            //my.ItemsSource = Global.EmployeeViewModel.Data;
            //Global.EmployeeViewModel.PropertyChanged += myChange; 
            //my.RefreshData();
        }

        public void myChange(object sender, PropertyChangedEventArgs e)
        {
            GridControl_Summary.RefreshData();
            return ;
        }
    }
}
