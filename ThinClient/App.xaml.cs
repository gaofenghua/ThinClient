using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.ObjectModel;

namespace ThinClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public AS_Employee_ViewModel _employee_ViewModel = new AS_Employee_ViewModel();
        public AS_Employee_ViewModel Employee_ViewModel 
        {
            get { return this._employee_ViewModel; }
            set { this._employee_ViewModel = value; }
        }
        void AppStartup(object sender, StartupEventArgs args)
        {
            
        }
    }
}
