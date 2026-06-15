namespace CalConnect.Api.Meetings;

/// <summary>
/// 外部日历同步服务
/// </summary>
public interface ICalendarSyncService
{
    Task SyncMeetingInvitation(Meeting meeting);
}