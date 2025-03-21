namespace AttendanceBot.Models;
public partial class TelegramUpdate
{
    public class User
    {
        public long Id { get; set; }         // Telegram User ID
        public string FirstName { get; set; } // Student's First Name
        public string LastName { get; set; }  // Student's Second Name (Last Name)
        public string Username { get; set; }  // Telegram Username (@username)
    }
}
