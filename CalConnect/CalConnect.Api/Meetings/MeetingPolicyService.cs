namespace CalConnect.Api.Meetings;

public class MeetingPolicyService
{
    public async Task<MeetingPolicy> Get(MeetingType type)
    {
        return new MeetingPolicy
        {
            MinDuration = TimeSpan.FromMinutes(15),
            MaxDuration = TimeSpan.FromMinutes(120)
        };
    }
}