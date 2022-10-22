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
using Microsoft.Data.SqlClient;
using Dapper;
using System.Data;
using Microsoft.IdentityModel.Tokens;

namespace IoTDevice
{
    public partial class MainWindow : Window
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; } = "OneDevice";
        public string Owner { get; set; } = "David";
        public string Type { get; set; } = "LightStrip";

        public readonly string ConnectionUrl = "http://localhost:7071/api/devices/connect";
        public readonly string ConnectionIoTHub = "HostName=EnIoTHubYo.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=7U1loLASWut2RKvjqaCH5XIz92xPrP3R4+E8wokeiOM=";
        public readonly string ConnectionDb = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\David!\\Documents\\IotDb.mdf;Integrated Security=True;Connect Timeout=30";

        public MainWindow()
        {
            InitializeComponent();
            Setup();
        }

        private void Setup()
        {
            using IDbConnection connection = new SqlConnection(ConnectionDb);
            var deviceId = connection.QueryFirstOrDefault<string>("SELECT DeviceId FROM DeviceInfo");
            if (string.IsNullOrEmpty(deviceId))
            {
                deviceId = Guid.NewGuid().ToString();
                connection.Execute("INSERT INTO DeviceInfo (DeviceId) VALUES (@DeviceId)", new { DeviceId = deviceId });
            }

            var deviceConnectionDb = connection.QueryFirstOrDefault<string>("SELECT ConnectionDb FROM DeviceInfo WHERE DeviceId = @DeviceId", new { DeviceId = deviceId });
            if (string.IsNullOrEmpty(deviceConnectionDb))
            {
               
            }
            else
            {

            }
        }

        private void Loop()
        {

        }
    }
}
