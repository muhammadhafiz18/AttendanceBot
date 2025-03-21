namespace AttendanceBot.Models;

public class Message
{
    public string? Text { get; set; } 
    public User? From { get; set; }
    public Location? Location { get; set; }
}