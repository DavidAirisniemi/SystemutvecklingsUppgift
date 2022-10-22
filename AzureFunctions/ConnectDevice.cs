using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Devices;

namespace AzureFunctions
{
    public static class ConnectDevice
    {
        private static readonly RegistryManager _registryManager =
            RegistryManager.CreateFromConnectionString(Environment.GetEnvironmentVariable("IoTHub"));
        [FunctionName("ConnectDevice")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "devices/connect")] HttpRequest req,
            ILogger log)
        {
            try
            {
                using var registryManager = RegistryManager.CreateFromConnectionString(Environment.GetEnvironmentVariable("IoTHub"));
                var device = await registryManager.GetDeviceAsync(req.Query["deviceId"]);

                //    if null   do this
                device ??= await registryManager.AddDeviceAsync(new Device(req.Query["deviceId"]));


                return new OkObjectResult($"{Environment.GetEnvironmentVariable("IoTHub").Split(";")};DeviceId={device.Id};SharedAccessKey={device.Authentication.SymmetricKey.PrimaryKey}");
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}
