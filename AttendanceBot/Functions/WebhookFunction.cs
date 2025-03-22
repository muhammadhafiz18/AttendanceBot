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
    private const double CENTERLATITUDE = 41.38150994089365;
    private const double CENTERLONGITUDE = 69.21664774464672;
    private const double RADIUSMETERS = 100;

    [Function("WebhookFunction")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
    {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        logger.LogInformation("Received raw request: {requestBody}", requestBody);

        var update = JsonSerializer.Deserialize<TelegramUpdate>(requestBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (update?.Message == null)
        {
            logger.LogInformation("Received non-message update. Full content: {Content}", 
                requestBody);
            return new OkObjectResult("Update processed");
        }

        var message = update.Message;
        long chatId = message.From?.Id ?? throw new InvalidOperationException("Message.From.Id is null");

        if (!string.IsNullOrEmpty(message.Text) && message.Text == "/start")
        {
            logger.LogInformation("User {ChatId} started the bot.", chatId);
            await telegramService.SendStartMessageAsync(chatId);
            return new OkObjectResult("Start message sent.");
        }

        if (message.Location != null)
        {
            logger.LogInformation("Received location from user {ChatId}.", chatId);

            bool isReplyToWelcome = message.ReplyToMessage?.Text == "Welcome! Press the button below to share your location.";

            if (!isReplyToWelcome)
            {
                await telegramService.SendMessageAsync(chatId, 
                    "Iltimos, lokatsiyani yuborish uchun \"📍 I am here\" tugmasidan foydalaning.");
                return new OkObjectResult("Invalid location type processed.");
            }

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

            bool isNear = IsWithinRadius(locationData.Latitude, locationData.Longitude, CENTERLATITUDE, CENTERLONGITUDE, RADIUSMETERS);

            string studentName = !string.IsNullOrEmpty(locationData.Username) ? $"@{locationData.Username}" 
                              : !string.IsNullOrEmpty(locationData.FirstName) ? locationData.FirstName 
                              : "Student";

            string responseMessage = isNear
                ? $"{studentName} o'quv markazga yetib keldi."
                : $"{studentName} o'quv markazda emassiz, iltimos o'quv markazga yetib borganingizda xabar yuboring.";

            await telegramService.SendMessageAsync(chatId, responseMessage);

            return new OkObjectResult("Location processed.");
        }

        logger.LogInformation(
            "Received unsupported message from user {ChatId}. Type: {MessageType}. Full content: {Content}", 
            chatId,
            GetMessageType(message),
            requestBody);

        return new OkObjectResult("Update processed");
    }

    /// <summary>
    /// Checks if a given location is within a specified radius from a central point.
    /// Uses the Haversine formula to calculate the great-circle distance.
    /// </summary>
    private static bool IsWithinRadius(double lat1, double lon1, double lat2, double lon2, double radiusMeters)
    {
        const double EarthRadiusMeters = 6371000;

        double dLat = ToRadians(lat2 - lat1);
        double dLon = ToRadians(lon2 - lon1);

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        double distance = EarthRadiusMeters * c;

        return distance <= radiusMeters;
    }

    private static double ToRadians(double degrees) => degrees * Math.PI / 180;

    private static string GetMessageType(Message message) =>
        message.Sticker != null ? "Sticker" :
        !string.IsNullOrEmpty(message.Text) ? "Text" :
        message.Photo != null ? "Photo" :
        message.Video != null ? "Video" :
        message.Voice != null ? "Voice" :
        message.Document != null ? "Document" :
        "Unknown";
}