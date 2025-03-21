using AttendanceBot.Abstractions;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Text;

namespace AttendanceBot.Services;

public class TelegramService(HttpClient httpClient, IConfiguration configuration) : ITelegramService
{
    private readonly string botToken = configuration["TelegramBotToken"] ?? "There is no bot token";

    public async Task SendLocationButtonAsync(long chatId)
    {
        var url = $"https://api.telegram.org/bot{botToken}/sendMessage";

        var message = new
        {
            chat_id = chatId,
            text = "Click the button below to send your current location:",
            reply_markup = new
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
                resize_keyboard = true,
                one_time_keyboard = true
            }
        };

        var json = JsonSerializer.Serialize(message);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        await httpClient.PostAsync(url, content);
    }
}
