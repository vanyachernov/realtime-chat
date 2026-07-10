namespace RealtimeChat.Application.Common.DTO;

public record MessageDto(Guid Id, Guid SenderId, string SenderName, Guid RoomId, string Content, DateTime SentAt);
