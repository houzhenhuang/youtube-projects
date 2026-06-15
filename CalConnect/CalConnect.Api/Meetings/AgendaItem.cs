namespace CalConnect.Api.Meetings;

/// <summary>
/// 议程项目
/// </summary>
public class AgendaItem
{
    public Guid Id { get; set; }

    public Guid MeetingId { get; set; }

    public string Title { get; set; }

    public TimeSpan Duration { get; set; }
}