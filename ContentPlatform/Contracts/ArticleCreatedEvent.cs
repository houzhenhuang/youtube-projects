using Curitis.EventBus;

namespace Contracts;

public class ArticleCreatedEvent: Event
{
    public Guid ArticleId { get; set; }

    public DateTime CreatedOnUtc { get; set; }
}