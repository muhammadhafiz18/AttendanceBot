using System.Text.Json;
using AttendanceBot.Abstractions;
using AttendanceBot.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AttendanceBot.Functions;

public class ReceiveLocationFunction(ILogger<ReceiveLocationFunction> logger, ILocationService locationService)
{
    [Function("ReceiveLocationFunction")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var update = JsonSerializer.Deserialize<TelegramUpdate>(requestBody);

            if (update?.Message?.Location != null)
            {
                var locationData = new LocationData
                {
                    UserId = update.Message.From!.Id,
                    Username = update.Message.From.Username,
                    FirstName = update.Message.From.FirstName,
                    LastName = update.Message.From.LastName,
                    Latitude = update.Message.Location.Latitude,
                    Longitude = update.Message.Location.Longitude,
                    Timestamp = DateTime.UtcNow
                };

                await locationService.LogLocationAsync(locationData);

                logger.LogInformation("Location received: {locationData.Latitude}, {locationData.Longitude} from {locationData.Username}", locationData.Latitude, locationData.Longitude, locationData.Username);

                return new OkObjectResult("Location logged successfully.");
            }

            return new BadRequestObjectResult("Invalid location data.");
        }
}