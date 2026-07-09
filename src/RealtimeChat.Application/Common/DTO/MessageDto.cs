namespace RealtimeChat.Application.Common.DTO;

public record MessageDto(Guid Id, Guid SenderId, Guid RoomId, string Content, DateTime SentAt);
