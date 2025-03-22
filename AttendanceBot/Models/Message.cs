namespace AttendanceBot.Models;
using System.Text.Json.Serialization;

public class Message
{
    [JsonPropertyName("message_id")]
    public long MessageId { get; set; }
    public string? Text { get; set; } 
    public User? From { get; set; }
    public Location? Location { get; set; }
    public int Date { get; set; }
    [JsonPropertyName("reply_to_message")]
    public Message? ReplyToMessage { get; set; }
    
    // Add these properties to detect message types
    public object? Sticker { get; set; }
    public object? Photo { get; set; }
    public object? Video { get; set; }
    public object? Voice { get; set; }
    public object? Document { get; set; }
}