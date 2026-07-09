namespace RealtimeChat.Domain.Entities;

public class Message
{
    public Guid Id { get; set; }
    public Guid SenderId { get; set; }
    public Guid RoomId { get; set; }
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Stored in UTC.
    /// </summary>
    public DateTime SentAt { get; set; }
}
