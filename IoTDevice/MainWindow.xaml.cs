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
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using Microsoft.Azure.Devices.Client;
using DotNetty.Transport.Channels;
using IoTDevice.Models;

namespace IoTDevice
{
    public partial class MainWindow : Window
    {
        public int Id { get; set; } = 0;
        public string DeviceName = "OneDevice";
        public string DeviceOwner = "David";
        public string DeviceType = "LightStrip";
        private string Location = "Bedroom";

        public readonly string ConnectionUrl = "http://localhost:7071/api/devices/connect";
        public readonly string ConnectionIoTHub = "HostName=EnIoTHubYo.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=7U1loLASWut2RKvjqaCH5XIz92xPrP3R4+E8wokeiOM=";
        public readonly string ConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\David!\\Documents\\IotDb.mdf;Integrated Security=True;Connect Timeout=30";

        private string device_ConnectionString = "";
        private DeviceClient deviceClient;
        private DeviceInfo deviceInfo;
        private bool LightState = false;
        private bool LightPrevState = false;
        private bool Connected = false;
        private int Interval = 1000;

        public MainWindow()
        {
            InitializeComponent();
            Setup().ConfigureAwait(false);
            Loop().ConfigureAwait(false);
        }

        private async Task Setup()
        {
            tbStateMessage.Text = "Initializing Device. Please wait...";

            using IDbConnection connection = new SqlConnection(ConnectionString);
            var deviceId = await connection.QueryFirstOrDefaultAsync<string>("SELECT DeviceId FROM DeviceInfo");
            if (string.IsNullOrEmpty(deviceId))
            {
                tbStateMessage.Text = "Generating new DeviceId";
                deviceId = Guid.NewGuid().ToString();
                await connection.ExecuteAsync("INSERT INTO DeviceInfo (DeviceId, DeviceName, DeviceType, Location, Owner) VALUES (@DeviceId, @DeviceName, @DeviceType, @Location, @Owner)", new { DeviceId = deviceId, DeviceName = "One Device", DeviceType = "Lightstrip", Location = "Bedroom", Owner = "David" }); ;
            }

            var deviceConnectionDb = await connection.QueryFirstOrDefaultAsync<string>("SELECT ConnectionString FROM DeviceInfo WHERE DeviceId = @DeviceId", new { DeviceId = deviceId });
            if (string.IsNullOrEmpty(deviceConnectionDb))
            {
                tbStateMessage.Text = "Initializing ConnectionString. Please wait...";
                using var http = new HttpClient();
                var result = await http.PostAsJsonAsync(Environment.GetEnvironmentVariable("ConnectionUrl"), new { DeviceId = deviceId});
                device_ConnectionString = await result.Content.ReadAsStringAsync();
                await connection.ExecuteAsync("Update DeviceInfo SET ConnectionString = @ConnectionString WHERE DeviceId = @DeviceId", new { DeviceId = deviceId, ConnectionString = device_ConnectionString });
            }

            deviceClient = DeviceClient.CreateFromConnectionString(device_ConnectionString);

            tbStateMessage.Text = "Updating Twin Properties. Please wait...";

            deviceInfo = await connection.QueryFirstOrDefaultAsync<DeviceInfo>("SELECT * FROM DeviceInfo WHERE DeviceId = @DeviceId", new { DeviceId = deviceId });

            var twinCollection = new TwinCollection();
            twinCollection["deviceName"] = deviceInfo.DeviceName;
            twinCollection["deviceOwner"] = deviceInfo.Owner;
            twinCollection["deviceType"] = deviceInfo.DeviceType;
            twinCollection["Location"] = deviceInfo.Location;
            twinCollection["LightState"] = LightState;

            await deviceClient.UpdateReportedPropertiesAsync(twinCollection);

            Connected = true;
            tbStateMessage.Text = "Device Connected";
        }

        private async Task Loop()
        {
            while (true)
            {
                if (Connected)
                {
                    if (LightState != LightPrevState)
                    {
                        LightPrevState = LightState;

                        var json = JsonConvert.SerializeObject(new { lightState = LightState });
                        var message = new Message(Encoding.UTF8.GetBytes(json));
                        message.Properties.Add("DeviceName", deviceInfo.DeviceName);
                        message.Properties.Add("deviceOwner", deviceInfo.Owner);
                        message.Properties.Add("deviceType", deviceInfo.DeviceType);
                        message.Properties.Add("Location", deviceInfo.Location);

                        await deviceClient.SendEventAsync(message);
                        tbStateMessage.Text = $"Message sent at {DateTime.Now}.";

                        var twinCollection = new TwinCollection();
                        twinCollection["LightState"] = LightState;
                        await deviceClient.UpdateReportedPropertiesAsync(twinCollection);
                    }
                }
                await Task.Delay(Interval);
            }
        }

        private void btnOnOffClick(object sender, RoutedEventArgs e)
        {
            LightState = !LightState;

            if (LightState)
                btnOnOff.Content = "Turn Off";
            else
                btnOnOff.Content = "Turn On";
        }
    }
}
