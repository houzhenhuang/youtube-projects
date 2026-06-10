using Curitis.EventBus;

namespace Contracts;

public sealed class ArticleDeletedEvent(Guid Id) : Event;