namespace AttendanceBot.Models;

public class LocationData
{
    public long UserId { get; set; }       
    public string? FirstName { get; set; }
    public string? LastName { get; set; }   
    public string? Username { get; set; }    
    public float Latitude { get; set; } 
    public float Longitude { get; set; }
    public DateTime Timestamp { get; set; }
}