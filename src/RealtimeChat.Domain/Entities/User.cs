namespace RealtimeChat.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Stored in UTC.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
