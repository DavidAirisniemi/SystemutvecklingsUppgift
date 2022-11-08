using IoTAdminApp.MVVM.Models;
using Microsoft.Azure.Devices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTAdminApp.MVVM.ViewModels
{
    public class BedroomViewModel
    {
        public IEnumerable<DeviceModel> DeviceItems => _deviceItems;
        private ObservableCollection<DeviceModel> _deviceItems;

        public BedroomViewModel()
        {
            _deviceItems = new ObservableCollection<DeviceModel>();
            AddDeviceItems().ConfigureAwait(false);
        }

        public async Task AddDeviceItems()
        {
            await Task.Delay(1000);
            _deviceItems = await GetDevicesAsync();
        }

        public async Task<ObservableCollection<DeviceModel>> GetDevicesAsync()
        {
            var devices = new ObservableCollection<DeviceModel>();

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
