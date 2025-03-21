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
        logger.LogInformation("Received raw request: {requestBody}", requestBody);

        TelegramUpdate? update;
        try
        {
            update = JsonSerializer.Deserialize<TelegramUpdate>(requestBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (update?.Message == null)
            {
                logger.LogWarning("Invalid update: Missing 'message' field.");
                return new BadRequestObjectResult("Invalid update: Missing 'message' field.");
            }
        }
        catch (JsonException ex)
        {
            logger.LogError("Failed to deserialize JSON: {Message}", ex.Message);
            return new BadRequestObjectResult("Invalid JSON format.");
        }

        var message = update.Message;
        long chatId = message.From!.Id;

        if (!string.IsNullOrEmpty(message.Text) && message.Text == "/start")
        {
            logger.LogInformation("User {ChatId} started the bot.", chatId);
            await telegramService.SendStartMessageAsync(chatId);
            return new OkObjectResult("Start message sent.");
        }

        if (message.Location != null)
        {
            logger.LogInformation("Received location from user {ChatId}.", chatId);
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
        if(!string.IsNullOrEmpty(message.Text))
        {
            logger.LogInformation("User {ChatId} sent smth. to the bot.", chatId);
            return new OkObjectResult("Nothing is sent");
        }

        return new BadRequestObjectResult("Invalid update received.");
    }
}