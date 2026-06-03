using ContentPlatform.Reporting.Api.Database;
using ContentPlatform.Reporting.Api.Entities;
using Contracts;
using Curitis.EventBus;

namespace ContentPlatform.Reporting.Api.EventHandlers;

public class ArticleCreatedEventHandler(ILogger<ArticleCreatedEventHandler> logger, ApplicationDbContext dbContext) : IEventHandler<ArticleCreatedEvent>
{
    public async Task HandleAsync(ArticleCreatedEvent @event, CancellationToken cancellationToken)
    {
        logger.LogInformation("文章创建完成：{ArticleId} {CreatedOnUtc}", @event.ArticleId, @event.CreatedOnUtc);

        await dbContext.Articles.AddAsync(new Article
        {
            Id = @event.ArticleId,
            CreatedOnUtc = @event.CreatedOnUtc
        }, cancellationToken);

        await dbContext.ArticleEvents.AddAsync(new ArticleEvent
        {
            ArticleId = @event.ArticleId,
            EventType = ArticleEventType.View,
            CreatedOnUtc = @event.CreatedOnUtc
        }, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

    }
}
