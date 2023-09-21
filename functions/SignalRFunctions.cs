using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace SignalRFunctions
{
    public static class SignalRFunctions
    {
        public static string warehouseid;
        public static string TimeInterval;
        public static int ShelfId;
        public static int SlotQuantity;
        public static string ShelfProduct;
        public static int ProductId;
        public static string ProductName;
        public static string ProductCategory;
        public static string ProductManufacturer;
        public static string ProductOfCustomer;
        public static string ProductImageURL;
        public static int BatteryUsageTimeOfRobot;
        public static int RemainingBatteryOfRobot;
        public static int BatteryTravelDistanceOfRobot;
        public static int ProductQuantity;
        public static string RobotCarryingProductName;
        public static int RobotCarryingProductQuantity;
        public static int OrderFullillment;


        [FunctionName("negotiate")]
        public static SignalRConnectionInfo GetSignalRInfo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req,
            [SignalRConnectionInfo(HubName = "dttelemetry")] SignalRConnectionInfo connectionInfo)
        {
            return connectionInfo;
        }

        [FunctionName("broadcast")]
        public static Task SendMessage(
            [EventGridTrigger] EventGridEvent eventGridEvent,
            [SignalR(HubName = "dttelemetry")] IAsyncCollector<SignalRMessage> signalRMessages,
            ILogger log)
        {
            JObject eventGridData = (JObject)JsonConvert.DeserializeObject(eventGridEvent.Data.ToString());
            if (eventGridEvent.EventType.Contains("telemetry"))
            {
                var data = eventGridData.SelectToken("data");

                var telemetryMessage = new Dictionary<object, object>();
                foreach (JProperty property in data.Children())
                {
                    log.LogInformation(property.Name + " - " + property.Value);
                    telemetryMessage.Add(property.Name, property.Value);
                }
                return signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "TelemetryMessage",
                    Arguments = new[] { telemetryMessage }
                });
            }
            else
            {
                try
                {
                    warehouseid = eventGridEvent.Subject;

                    var data = eventGridData.SelectToken("data");
                    var patch = data.SelectToken("patch");

                    var property = new Dictionary<object, object>
                    {
                        {"warehouseid", warehouseid },
                        {"TimeInterval", TimeInterval },
                        {"ShelfId", ShelfId },
                        {"SlotQuantity", SlotQuantity },
                        {"ShelfProduct", ShelfProduct },
                        {"ProductId", ProductId },
                        {"ProductName", ProductName },
                        {"ProductCategory", ProductCategory },
                        {"ProductManufacturer", ProductManufacturer },
                        {"ProductOfCustomer", ProductOfCustomer },
                        {"ProductImageURL", ProductImageURL },
                        {"BatteryUsageTimeOfRobot", BatteryUsageTimeOfRobot },
                        {"RemainingBatteryOfRobot", RemainingBatteryOfRobot },
                        {"BatteryTravelDistanceOfRobot", BatteryTravelDistanceOfRobot },
                        {"ProductQuantity", ProductQuantity },
                        {"RobotCarryingProductName", RobotCarryingProductName },
                        {"RobotCarryingProductQuantity", RobotCarryingProductQuantity },
                        {"OrderFullillment", OrderFullillment }
                    };
                    return signalRMessages.AddAsync(
                        new SignalRMessage
                        {
                            Target = "PropertyMessage",
                            Arguments = new[] { property }
                        });
                }
                catch (Exception e)
                {
                    log.LogInformation(e.Message);
                    return null;
                }
            }

        }
    }
}