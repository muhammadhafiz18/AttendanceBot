namespace AttendanceBot.Abstractions;

public interface ITelegramService
{
    Task SendLocationButtonAsync(long chatId);
}