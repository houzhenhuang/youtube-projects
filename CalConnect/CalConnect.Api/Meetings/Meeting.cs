namespace CalConnect.Api.Meetings;

public class Meeting
{
    public Guid Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public TimeSpan Duration { get; set; }
    public string Location { get; set; }

    public MeetingType Type { get; set; }

    public List<AgendaItem> AgendaItems { get; set; } = [];

    public List<Participant> Participants { get; set; } = [];
}