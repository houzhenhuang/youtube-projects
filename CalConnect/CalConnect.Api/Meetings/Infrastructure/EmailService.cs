namespace CalConnect.Api.Meetings.Infrastructure;

public class EmailService : IEmailService
{
    public Task SendMeetingInvitation(Guid userId, Meeting meeting)
    {
        return Task.CompletedTask;
    }
}