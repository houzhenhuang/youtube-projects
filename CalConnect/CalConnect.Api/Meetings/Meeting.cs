using CalConnect.Api.Extensions;
using static CalConnect.Api.Meetings.CreateMeeting;

namespace CalConnect.Api.Meetings;

public class Meeting
{
    private readonly List<Participant> _participants = [];
    private readonly List<AgendaItem> _agendaItems = [];
    private Meeting()
    {
    }

    public Guid Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public TimeSpan Duration { get; set; }
    public string Location { get; set; }

    public MeetingType Type { get; set; }

    public IReadOnlyCollection<AgendaItem> AgendaItems => _agendaItems;

    /// <summary>
    /// 会议参与者
    /// </summary>
    public IReadOnlyCollection<Participant> Participants => _participants.ToList();

    public static Meeting Create(
        string title,
        string description,
        DateTime startTime,
        TimeSpan duration,
        string location,
        MeetingType type,
        MeetingPolicy meetingPolicy)
    {
        var meeting = new Meeting
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = description,
            StartTime = startTime,
            EndTime = startTime.Add(duration),
            Duration = duration,
            Location = location,
            Type = type
        };

        if (!(meeting.Duration >= meetingPolicy.MinDuration &&
              meeting.Duration <= meetingPolicy.MaxDuration))
        {
            throw new DomainException($"{meeting.Type} 会议类型的会议持续时间无效。");
        }

        return meeting;
    }


    public void AddAgendaItem(string title, TimeSpan duration)
    {
        var totalAgendaDuration = new TimeSpan(_agendaItems.Sum(x => x.Duration.Ticks)) + duration;
        if (totalAgendaDuration > Duration)
        {
            throw new DomainException("议程总时长超过了会议时长。");
        }
        _agendaItems.Add(new AgendaItem
        {
            Title = title,
            Duration = duration
        });
    }

    public void AddParticipant(Guid userId, ParticipantRole role)
    {
        if (!CanAddParticipantWithRole(role))
        {
            throw new DomainException($"无法将角色为 {role} 的参与者添加到此会议类型。");
        }

        _participants.Add(new Participant
        {
            UserId = userId,
            Role = role,
            Response = role == ParticipantRole.Organizer ? ParticipantResponse.Accepted : ParticipantResponse.Pending
        });
    }

    private bool CanAddParticipantWithRole(ParticipantRole role)
    {
        // Check if the meeting has reached its maximum capacity
        int maxParticipants = Type switch
        {
            MeetingType.Standard => 20,
            MeetingType.Workshop => 30,
            MeetingType.DecisionMaking => 10,
            _ => throw new InvalidOperationException($"未知的会议类型：{Type}")
        };

        if (_participants.Count >= maxParticipants)
        {
            return false;
        }

        // Check if the role is allowed for this meeting type
        bool isRoleAllowed = Type switch
        {
            MeetingType.Standard => true,
            MeetingType.Workshop => role != ParticipantRole.DecisionMaker,
            MeetingType.DecisionMaking => role == ParticipantRole.DecisionMaker,
            _ => throw new InvalidOperationException($"未知的会议类型：{Type}")
        };

        if (!isRoleAllowed)
        {
            return false;
        }

        // Check if there's already a participant with this role (for unique roles)
        if ((role == ParticipantRole.Organizer || role == ParticipantRole.Facilitator) &&
            _participants.Any(p => p.Role == role))
        {
            return false;
        }

        return true;
    }
}