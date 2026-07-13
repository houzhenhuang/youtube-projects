namespace Notes.Api.DTOs;

public sealed record NoteDto(Guid Id, string Content, DateTime CreatedAt);

public sealed record CreateNoteRequest(string Content);