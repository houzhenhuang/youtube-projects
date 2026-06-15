namespace CalConnect.Api.Meetings;

public interface IEmailService
{
    Task SendMeetingInvitation(Guid userId, Meeting meeting);
}