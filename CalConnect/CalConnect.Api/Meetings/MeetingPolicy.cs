namespace CalConnect.Api.Meetings;

public class MeetingPolicy
{
    public TimeSpan MinDuration { get; set; }
    public TimeSpan MaxDuration { get; set; }

    public MeetingType MeetingType { get; set; }
}