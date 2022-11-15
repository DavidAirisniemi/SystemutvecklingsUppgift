using IoTAdminApp.MVVM.Models;
using Microsoft.Azure.Devices;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
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
using Dapper;

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

        private async void OnOffSwitch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as CheckBox;
                var deviceItem = (DeviceModel)button!.DataContext;

                using ServiceClient serviceClient = ServiceClient.CreateFromConnectionString("HostName=EnIoTHubYo.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=7U1loLASWut2RKvjqaCH5XIz92xPrP3R4+E8wokeiOM=");
                var directMethod = new CloudToDeviceMethod("ChangeLightstate");
                var result = await serviceClient.InvokeDeviceMethodAsync(deviceItem.Id, directMethod);
            }
            catch { }
        }

        private async void DeleteDevice_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var deviceItem = (DeviceModel)button!.DataContext;

                using ServiceClient serviceClient = ServiceClient.CreateFromConnectionString("HostName=EnIoTHubYo.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=7U1loLASWut2RKvjqaCH5XIz92xPrP3R4+E8wokeiOM=");
                var directMethod = new CloudToDeviceMethod("DeleteDevice");
                serviceClient.InvokeDeviceMethodAsync(deviceItem.Id, directMethod);

                using var registryManager = RegistryManager.CreateFromConnectionString("HostName=EnIoTHubYo.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=7U1loLASWut2RKvjqaCH5XIz92xPrP3R4+E8wokeiOM=");
                await registryManager.RemoveDeviceAsync(deviceItem.Id);
                using IDbConnection connection = new SqlConnection("Server=tcp:enserveryo.database.windows.net,1433;Initial Catalog=EnBankYo;Persist Security Info=False;User ID=Adminloginyo;Password=Lösenordyo1;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
                await connection.ExecuteAsync($"DELETE FROM DeviceInfo WHERE DeviceId = @Id", new { Id = deviceItem.Id });

            }
            catch { }
        }
    }
}
