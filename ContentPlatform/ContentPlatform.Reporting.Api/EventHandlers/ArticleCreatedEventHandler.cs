using Contracts;
using Curitis.EventBus;

namespace ContentPlatform.Reporting.Api.EventHandlers;

public class ArticleCreatedEventHandler(ILogger<ArticleCreatedEventHandler> logger) : IEventHandler<ArticleCreatedEvent>
{
    public Task HandleAsync(ArticleCreatedEvent @event, CancellationToken cancellationToken)
    {
        logger.LogInformation("文章创建完成：{EventId} {CreatedOnUtc}", @event.Id, @event.CreatedOnUtc);

        return Task.CompletedTask;
    }
}
