namespace AttendanceBot.Models;

public class Message
{
    public long MessageId { get; set; }
    public string? Text { get; set; } 
    public User? From { get; set; }
    public Location? Location { get; set; }
    public int Date { get; set; }
    
    // Add these properties to detect message types
    public object? Sticker { get; set; }
    public object? Photo { get; set; }
    public object? Video { get; set; }
    public object? Voice { get; set; }
    public object? Document { get; set; }
}