using Carter;
using ContentPlatform.Api.Database;
using Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace ContentPlatform.Api.Articles;

public static class DeleteArticle
{
    public class Command : IRequest<Result>
    {
        public Guid Id { get; set; }
    }

    internal sealed class Handler : IRequestHandler<Command, Result>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly Curitis.EventBus.IPublisher _publisher;

        public Handler(ApplicationDbContext dbContext, Curitis.EventBus.IPublisher publisher)
        {
            _dbContext = dbContext;
            _publisher = publisher;
        }

        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var article = await _dbContext
                .Articles
                .Where(article => article.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (article is null)
            {
                return Result.Failure(new Error(
                    "GetArticle.Null",
                    "未找到指定ID的文章"));
            }

            _dbContext.Remove(article);

            await _dbContext.SaveChangesAsync(cancellationToken);

            await _publisher.PublishAsync(new ArticleDeletedEvent(article.Id), cancellationToken);

            return Result.Success();
        }
    }
}

public class DeleteArticleEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/articles/{id}", async (Guid id, ISender sender) =>
        {
            var command = new DeleteArticle.Command { Id = id };

            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.NotFound(result.Error);
            }

            return Results.Ok();
        });
    }
}