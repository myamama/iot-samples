using System;
using System.Threading.Tasks;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Azure.Devices;

namespace IoTSamplesDps.Function
{
    public static class EventGridChildCreatedTrigger
    {
        private static string iotHubConnectionString;
       
        [FunctionName("EventGridChildCreated")]
        public static async Task EventGridChildCreated([EventGridTrigger]EventGridEvent eventGridEvent, ILogger log)
        {
            iotHubConnectionString = System.Environment.GetEnvironmentVariable("iotHubConnectionString", EnvironmentVariableTarget.Process);

            //log.LogInformation(eventGridEvent.Data.ToString());
            
            var data = eventGridEvent.Data as JObject;
            if(data.ContainsKey("deviceId") && data["deviceId"].ToString().Contains("-"))
            {
                RegistryManager registryManager = RegistryManager.CreateFromConnectionString(iotHubConnectionString);
                //get the parent and child, then assign parent Scope to child device - exception management missing
                string childDeviceId = data["deviceId"].ToString();
                string parentDeviceId = childDeviceId.Substring(childDeviceId.IndexOf("-")+1);
                log.LogInformation($"DeviceId: {childDeviceId}");
                log.LogInformation($"Parent device id: {parentDeviceId}");

                var parentDevice = await registryManager.GetDeviceAsync(parentDeviceId).ConfigureAwait(false);
                var childDevice = await registryManager.GetDeviceAsync(childDeviceId).ConfigureAwait(false);
                
                if(parentDevice != null && parentDevice.Capabilities.IotEdge)
                {
                    childDevice.Scope = parentDevice.Scope;
                    await registryManager.UpdateDeviceAsync(childDevice).ConfigureAwait(false);
                }

                log.LogInformation("updated the device to be parent of Edge (naming by convention)");

            }            

        }
    }
}
