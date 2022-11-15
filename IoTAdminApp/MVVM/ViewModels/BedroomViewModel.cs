using IoTAdminApp.MVVM.Models;
using Microsoft.Azure.Devices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace IoTAdminApp.MVVM.ViewModels
{
    public class BedroomViewModel
    {
        public IEnumerable<DeviceModel> DeviceItems => _deviceItems;
        private ObservableCollection<DeviceModel> _deviceItems;
        private DispatcherTimer _timer;

        public BedroomViewModel()
        {
            _deviceItems = new ObservableCollection<DeviceModel>();
            AddDeviceItems().ConfigureAwait(false);
            SetTimer(TimeSpan.FromSeconds(10));
        }

        private void SetTimer(TimeSpan interval)
        {
            _timer = new DispatcherTimer()
            {
                Interval = interval
            };
            _timer.Tick += new EventHandler(timer_tick);
            _timer.Start();
        }

        private async void timer_tick(object? sender, EventArgs e)
        {
            await AddDeviceItems();

            var removeList = new List<DeviceModel>();
            using var registryManager = RegistryManager.CreateFromConnectionString("HostName=EnIoTHubYo.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=7U1loLASWut2RKvjqaCH5XIz92xPrP3R4+E8wokeiOM=");
            foreach (var item in _deviceItems)
            {
                var device = await registryManager.GetDeviceAsync(item.Id);
                if (device == null)
                    removeList.Add(item);
            }

            foreach (var device in removeList)
            {
                _deviceItems.Remove(device);
            }

        }

        public async Task AddDeviceItems()
        {
            var result = await GetDevicesAsync();

            result.ForEach(device =>
            {
                var item = _deviceItems?.FirstOrDefault(x => x.Id == device.Id);
                if (item == null)
                {
                    _deviceItems?.Add(device);
                }
                else
                {
                    var index = _deviceItems!.IndexOf(item);
                    _deviceItems[index] = device;
                }
            });
        }

        public async Task<List<DeviceModel>> GetDevicesAsync()
        {
            var devices = new List<DeviceModel>();

            try
            {
                using var registryManager = RegistryManager.CreateFromConnectionString("HostName=EnIoTHubYo.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=7U1loLASWut2RKvjqaCH5XIz92xPrP3R4+E8wokeiOM=");
                var result = registryManager.CreateQuery("SELECT * FROM devices");

                if (result.HasMoreResults)
                {
                    foreach (var twin in await result.GetNextAsTwinAsync())
                    {
                        var device = new DeviceModel
                        {
                            Id = twin.DeviceId
                        };

                        try { device.DeviceName = twin.Properties.Reported["deviceName"].ToString(); }
                        catch { }
                        try { device.Location = twin.Properties.Reported["Location"].ToString(); }
                        catch { }

                        devices.Add(device);
                    }
                }
            }
            catch { }

            return devices;
        }
    }
}
