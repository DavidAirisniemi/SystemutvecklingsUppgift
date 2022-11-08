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

namespace IoTAdminApp.Components
{
    /// <summary>
    /// Interaction logic for DeviceTile.xaml
    /// </summary>
    public partial class DeviceTile : UserControl
    {
        public static readonly DependencyProperty IdProperty = DependencyProperty.Register("Id", typeof(string), typeof(DeviceTile));
        public string Id
        {
            get { return (string)GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }

        public static readonly DependencyProperty DeviceNameProperty = DependencyProperty.Register("DeviceName", typeof(string), typeof(DeviceTile));
        public string DeviceName
        {
            get { return (string)GetValue(DeviceNameProperty); }
            set { SetValue(DeviceNameProperty, value); }
        }

        public static readonly DependencyProperty LocationProperty = DependencyProperty.Register("Location", typeof(string), typeof(DeviceTile));
        public string Location
        {
            get { return (string)GetValue(LocationProperty); }
            set { SetValue(LocationProperty, value); }
        }

        public DeviceTile()
        {
            InitializeComponent();
        }

        private void OnOffSwitch_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
