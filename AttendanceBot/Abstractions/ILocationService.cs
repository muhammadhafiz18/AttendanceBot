using AttendanceBot.Models;

namespace AttendanceBot.Abstractions;
public interface ILocationService
{
    Task LogLocationAsync(LocationData locationData);
}