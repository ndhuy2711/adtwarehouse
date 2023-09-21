using Azure;
using Azure.Core.Pipeline;
using Azure.DigitalTwins.Core;
using Azure.Identity;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Collections.Generic;

namespace My.Function
{
    // This class processes telemetry events from IoT Hub, reads temperature of a device
    // and sets the "Temperature" property of the device with the value of the telemetry.
    public class telemetryfunction
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private static string adtServiceUrl = Environment.GetEnvironmentVariable("ADT_SERVICE_URL");

        [FunctionName("telemetryfunction")]
        public async void Run([EventGridTrigger] EventGridEvent eventGridEvent, ILogger log)
        {
            try
            {
                // After this is deployed, you need to turn the Managed Identity Status to "On",
                // Grab Object Id of the function and assigned "Azure Digital Twins Owner (Preview)" role
                // to this function identity in order for this function to be authorized on ADT APIs.
                //Authenticate with Digital Twins
                var credentials = new DefaultAzureCredential();
                log.LogInformation(credentials.ToString());
                DigitalTwinsClient client = new DigitalTwinsClient(
                    new Uri(adtServiceUrl), credentials, new DigitalTwinsClientOptions
                    { Transport = new HttpClientTransport(httpClient) });
                log.LogInformation($"ADT service client connection created.");

                if (eventGridEvent != null && eventGridEvent.Data != null)
                {

                    JObject deviceMessage = (JObject)JsonConvert.DeserializeObject(eventGridEvent.Data.ToString());
                    string deviceId = (string)deviceMessage["systemProperties"]["iothub-connection-device-id"];
                    var ID = deviceMessage["body"]["warehouseid"] != null ? deviceMessage["body"]["warehouseid"] : "";
                    var TimeInterval = deviceMessage["body"]["TimeInterval"] != null ? deviceMessage["body"]["TimeInterval"] : "";
                    var ShelfId = deviceMessage["body"]["ShelfId"] != null ? deviceMessage["body"]["ShelfId"] : "";
                    var SlotQuantity = deviceMessage["body"]["SlotQuantity"] != null ? deviceMessage["body"]["SlotQuantity"] : 0 ;
                    var ShelfProduct = deviceMessage["body"]["ShelfProduct"] != null ? deviceMessage["body"]["ShelfProduct"] : "";
                    var ProductId = deviceMessage["body"]["ProductId"] != null ? deviceMessage["body"]["ProductId"] : 0;
                    var ProductName = deviceMessage["body"]["ProductName"] != null ? deviceMessage["body"]["ProductName"] : "" ;
                    var ProductCategory = deviceMessage["body"]["ProductCategory"] != null ? deviceMessage["body"]["ProductCategory"] : "";
                    var ProductManufacturer = deviceMessage["body"]["ProductManufacturer"] != null ? deviceMessage["body"]["ProductManufacturer"] : "" ;
                    var ProductOfCustomer = deviceMessage["body"]["ProductOfCustomer"] != null ? deviceMessage["body"]["ProductOfCustomer"] : "";
                    var ProductImageURL = deviceMessage["body"]["ProductImageURL"] != null ? deviceMessage["body"]["ProductImageURL"] : "";
                    var BatteryUsageTimeOfRobot = deviceMessage["body"]["BatteryUsageTimeOfRobot"] != null ? deviceMessage["body"]["BatteryUsageTimeOfRobot"] : 0;
                    var RemainingBatteryOfRobot = deviceMessage["body"]["RemainingBatteryOfRobot"] != null ? deviceMessage["body"]["RemainingBatteryOfRobot"] : 0;
                    var BatteryTravelDistanceOfRobot = deviceMessage["body"]["BatteryTravelDistanceOfRobot"] != null ? deviceMessage["body"]["BatteryTravelDistanceOfRobot"] : 0;
                    var ProductQuantity = deviceMessage["body"]["ProductQuantity"] != null ? deviceMessage["body"]["ProductQuantity"] : 0 ;
                    var RobotCarryingProductName = deviceMessage["body"]["RobotCarryingProductName"] != null ? deviceMessage["body"]["RobotCarryingProductName"] : "" ;
                    var RobotCarryingProductQuantity = deviceMessage["body"]["RobotCarryingProductQuantity"] != null ? deviceMessage["body"]["RobotCarryingProductQuantity"] : 0  ;
                    var OrderFullillment = deviceMessage["body"]["OrderFullillment"] != null ? deviceMessage["body"]["OrderFullillment"] : 0;


                    log.LogInformation($"Device:{deviceId} Device Id is: {ID}");
                    log.LogInformation($"Device:{deviceId} Time interval is: {TimeInterval}");
                    log.LogInformation($"Device:{deviceId} SlotQuantity is: {SlotQuantity}");
                    log.LogInformation($"Device:{deviceId} ShelfProduct is: {ShelfProduct}");
                    log.LogInformation($"Device:{deviceId} ShelfId is: {ShelfId}");
                    log.LogInformation($"Device:{deviceId} ProductId is: {ProductId}");
                    log.LogInformation($"Device: {deviceId} ProductName is: {ProductName}");
                    log.LogInformation($"Device: {deviceId} ProductCategory is: {ProductCategory}");
                    log.LogInformation($"Device: {deviceId} ProductManufacturer is: {ProductManufacturer}");
                    log.LogInformation($"Device: {deviceId} ProductOfCustomer is: {ProductOfCustomer}");
                    log.LogInformation($"Device: {deviceId} ProductCategory is: {ProductImageURL}");
                    log.LogInformation($"Device: {deviceId} BatteryUsageTimeOfRobot is: {BatteryUsageTimeOfRobot}");
                    log.LogInformation($"Device: {deviceId} RemainingBatteryOfRobot is: {RemainingBatteryOfRobot}");
                    log.LogInformation($"Device: {deviceId} BatteryTravelDistanceOfRobot is: {BatteryTravelDistanceOfRobot}");
                    log.LogInformation($"Device: {deviceId} ProductQuantity is: {ProductQuantity}");
                    log.LogInformation($"Device: {deviceId} RobotCarryingProductName is: {RobotCarryingProductName}");
                    log.LogInformation($"Device: {deviceId} RobotCarryingProductQuantity is: {RobotCarryingProductQuantity}");
                    log.LogInformation($"Device: {deviceId} OrderFullillment is: {OrderFullillment}");

                    var updateProperty = new JsonPatchDocument();
                    var turbineTelemetry = new Dictionary<string, Object>()
                    {
                        ["warehouseid"] = ID,
                        ["TimeInterval"] = TimeInterval,
                        ["ShelfId"] = ShelfId,
                        ["SlotQuantity"] = SlotQuantity,
                        ["ShelfProduct"] = ShelfProduct,
                        ["ProductId"] = ProductId,
                        ["ProductName"] = ProductName,
                        ["ProductCategory"] = ProductCategory,
                        ["ProductManufacturer"] = ProductManufacturer,
                        ["ProductOfCustomer"] = ProductOfCustomer,
                        ["ProductImageURL"] = ProductImageURL,
                        ["BatteryUsageTimeOfRobot"] = BatteryUsageTimeOfRobot,
                        ["RemainingBatteryOfRobot"] = RemainingBatteryOfRobot,
                        ["BatteryTravelDistanceOfRobot"] = BatteryTravelDistanceOfRobot,
                        ["ProductQuantity"] = ProductQuantity,
                        ["RobotCarryingProductName"] = RobotCarryingProductName,
                        ["RobotCarryingProductQuantity"] = RobotCarryingProductQuantity,
                        ["OrderFullillment"] = OrderFullillment

                    };
                    updateProperty.AppendReplace("/warehouseid", ID.Value<string>());
                    updateProperty.AppendReplace("/TimeInterval", TimeInterval.Value<string>());
                    updateProperty.AppendReplace("/ShelfId", ShelfId.Value<int>());
                    updateProperty.AppendReplace("/SlotQuantity", SlotQuantity.Value<int>());
                    updateProperty.AppendReplace("/ShelfProduct", ShelfProduct.Value<string>());
                    updateProperty.AppendReplace("/ProductId", ProductId.Value<int>());
                    updateProperty.AppendReplace("/ProductName", ProductName.Value<string>());
                    updateProperty.AppendReplace("/ProductCategory", ProductCategory.Value<string>());
                    updateProperty.AppendReplace("/ProductManufacturer", ProductManufacturer.Value<string>());
                    updateProperty.AppendReplace("/ProductOfCustomer", ProductOfCustomer.Value<string>());
                    updateProperty.AppendReplace("/ProductImageURL", ProductImageURL.Value<string>());
                    updateProperty.AppendReplace("/BatteryUsageTimeOfRobot", BatteryUsageTimeOfRobot.Value<int>());
                    updateProperty.AppendReplace("/RemainingBatteryOfRobot", RemainingBatteryOfRobot.Value<int>());
                    updateProperty.AppendReplace("/BatteryTravelDistanceOfRobot", BatteryTravelDistanceOfRobot.Value<int>());
                    updateProperty.AppendReplace("/ProductQuantity", ProductQuantity.Value<int>());
                    updateProperty.AppendReplace("/RobotCarryingProductName", RobotCarryingProductName.Value<string>());
                    updateProperty.AppendReplace("/RobotCarryingProductQuantity", RobotCarryingProductQuantity.Value<int>());
                    updateProperty.AppendReplace("/OrderFullillment", OrderFullillment.Value<int>());

                    log.LogInformation(updateProperty.ToString());
                    try
                    {
                        await client.PublishTelemetryAsync(deviceId, Guid.NewGuid().ToString(), JsonConvert.SerializeObject(turbineTelemetry));
                        await client.UpdateDigitalTwinAsync(deviceId, updateProperty);
                    }
                    catch (Exception e)
                    {
                        log.LogInformation(e.Message);
                    }
                }
            }
            catch (Exception e)
            {
                log.LogInformation(e.Message);
            }
        }
    }
}