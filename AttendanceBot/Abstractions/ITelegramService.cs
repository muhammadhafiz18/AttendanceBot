using AttendanceBot.Models;

namespace AttendanceBot.Abstractions;

public interface ITelegramService
{
    Task SendStartMessageAsync(long chatId);
    Task ForwardLocationToAzureAsync(LocationData locationData);
    Task SendMessageAsync(long chatId, string message);
}