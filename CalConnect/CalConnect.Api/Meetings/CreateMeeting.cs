using CalConnect.Api.Database;
using CalConnect.Api.Users;
using Microsoft.EntityFrameworkCore;

namespace CalConnect.Api.Meetings;

internal sealed class CreateMeeting(
    ApplicationDbContext context,
    IEmailService emailService,
    ICalendarSyncService calendarSyncService,
    MeetingPolicyService meetingPolicyService
    )
{
    public sealed record Request(
        string Title,
        string Description,
        DateTime StartTime,
        TimeSpan Duration,
        string Location,
        MeetingType Type,
        List<ParticipantRequest> Participants,
        List<AgendaItemRequest> AgendaItems,
        Guid OrganizerId);

    public sealed record ParticipantRequest(Guid UserId, ParticipantRole Role);

    public sealed record AgendaItemRequest(string Title, TimeSpan Duration);

    public async Task<Guid> Handle(Request request)
    {
        User? organizer = await context.Users.FindAsync(request.OrganizerId);
        if (organizer is null)
        {
            throw new ApplicationException("Organizer not found");
        }

        MeetingPolicy meetingPolicy = await meetingPolicyService.Get(request.Type);

        var meeting = Meeting.Create(
            request.Title,
            request.Description,
            request.StartTime,
            request.Duration,
            request.Location,
            request.Type,
            meetingPolicy
        );

        if (await CheckTimeConflicts(organizer.Id, meeting))
        {
            throw new ApplicationException("组织者与另一次会议有时间冲突。");
        }

        meeting.AddParticipant(organizer.Id, ParticipantRole.Organizer);

        foreach (ParticipantRequest participantRequest in request.Participants)
        {
            User? participant = await context.Users.FindAsync(participantRequest.UserId);
            if (participant is null)
            {
                throw new ApplicationException($"未找到 ID 为 {participantRequest.UserId} 的参与者。");
            }

            if (await CheckTimeConflicts(participant.Id, meeting))
            {
                throw new ApplicationException($"参与者 {participantRequest.UserId} 与另一次会议有时间冲突。");
            }

            meeting.AddParticipant(participant.Id, participantRequest.Role);
        }


        foreach (AgendaItemRequest agendaItemRequest in request.AgendaItems)
        {
            meeting.AddAgendaItem(agendaItemRequest.Title, agendaItemRequest.Duration);
        }

        context.Meetings.Add(meeting);

        await context.SaveChangesAsync();

        foreach (Participant participant in meeting.Participants)
        {
            await emailService.SendMeetingInvitation(participant.UserId, meeting);
        }

        await calendarSyncService.SyncMeetingInvitation(meeting);

        return meeting.Id;
    }


    /// <summary>
    /// 检查时间冲突
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="meeting"></param>
    /// <returns></returns>
    private async Task<bool> CheckTimeConflicts(Guid userId, Meeting meeting)
    {
        return await context.Meetings
            .Where(m => m.Participants.Any(p => p.UserId == userId))
            .Where(m => m.StartTime < meeting.EndTime && m.EndTime > meeting.StartTime)
            .AnyAsync();
    }
}