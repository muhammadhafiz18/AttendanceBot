using AttendanceBot.Abstractions;
using AttendanceBot.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AttendanceBot.Functions;

public class WebhookFunction(ILogger<WebhookFunction> logger, ITelegramService telegramService)
{
    [Function("WebhookFunction")]
    public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
    {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var update = JsonSerializer.Deserialize<TelegramUpdate>(requestBody);

        if (update?.Message != null)
        {
            var message = update.Message;
            long chatId = message.From!.Id;

            if (message.Text == "/start")
            {
                logger.LogInformation("User {chatId} started the bot.", chatId);
                await telegramService.SendStartMessageAsync(chatId);
                return new OkObjectResult("Start message sent.");
            }

            if (message.Location != null)
            {
                logger.LogInformation("Received location from user {chatId}.", chatId);
                var locationData = new LocationData
                {
                    UserId = message.From.Id,
                    Username = message.From.Username,
                    FirstName = message.From.FirstName,
                    LastName = message.From.LastName,
                    Latitude = message.Location.Latitude,
                    Longitude = message.Location.Longitude,
                    Timestamp = DateTime.UtcNow
                };

                await telegramService.ForwardLocationToAzureAsync(locationData);
                return new OkObjectResult("Location forwarded.");
            }
        }
        return new BadRequestObjectResult("Invalid update received.");
    }
}