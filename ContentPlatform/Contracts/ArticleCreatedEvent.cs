using Curitis.EventBus;

namespace Contracts;

public class ArticleCreatedEvent: Event
{
    public DateTime CreatedOnUtc { get; set; }
}