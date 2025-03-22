using AttendanceBot.Abstractions;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Text;
using AttendanceBot.Models;

namespace AttendanceBot.Services;

public class TelegramService(HttpClient httpClient, IConfiguration configuration) : ITelegramService
{
    private readonly string botToken = configuration["TelegramBotToken"] ?? "There is no bot token";
    private readonly string azureFunctionUrl = configuration["AzureFunctionUrl"] ?? "There is no Azure Url here";

    public async Task SendStartMessageAsync(long chatId)
    {
        var keyboard = new
        {
            keyboard = new[]
            {
                    new[]
                    {
                        new
                        {
                            text = "📍 I am here",
                            request_location = true
                        }
                    }
                },
            resize_keyboard = true
        };

        var payload = new
        {
            chat_id = chatId,
            text = "Welcome! Press the button below to share your location.",
            reply_markup = keyboard
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        await httpClient.PostAsync($"https://api.telegram.org/bot{botToken}/sendMessage", content);
    }

    public async Task ForwardLocationToAzureAsync(LocationData locationData)
    {
        var json = JsonSerializer.Serialize(locationData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        await httpClient.PostAsync(azureFunctionUrl, content);
    }

    public async Task SendMessageAsync(long chatId, string message)
    {
        var payload = new
        {
            chat_id = chatId,
            text = message
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        await httpClient.PostAsync($"https://api.telegram.org/bot{botToken}/sendMessage", content);
    }
}
