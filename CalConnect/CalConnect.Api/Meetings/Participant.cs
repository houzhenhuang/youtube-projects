namespace CalConnect.Api.Meetings;

/// <summary>
/// 会议参与者
/// </summary>
public class Participant
{
    public Guid Id { get; set; }

    public Guid MeetingId { get; set; }

    public Guid UserId { get; set; }
    public ParticipantRole Role { get; set; }

    public ParticipantResponse Response { get; set; }
}