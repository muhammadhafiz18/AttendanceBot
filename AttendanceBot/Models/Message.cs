namespace AttendanceBot.Models;
public partial class TelegramUpdate
{
    public class Message
    {
        public User From { get; set; }
        public Location Location { get; set; }
    }
}
