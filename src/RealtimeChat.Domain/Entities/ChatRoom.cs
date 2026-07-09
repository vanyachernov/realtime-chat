namespace RealtimeChat.Domain.Entities;

public class ChatRoom
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
