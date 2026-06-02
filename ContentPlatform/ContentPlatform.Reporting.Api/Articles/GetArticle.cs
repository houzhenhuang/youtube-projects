using Carter;
using ContentPlatform.Reporting.Api.Database;
using ContentPlatform.Reporting.Api.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace ContentPlatform.Reporting.Api.Articles;

public static class GetArticle
{
    public class Query : IRequest<Result<Response>>
    {
        public Guid Id { get; set; }
    }
    public class Response
    {
        public Guid Id { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime? PublishedOnUtc { get; set; }
        public List<ArticleEventResponse> Events { get; set; } = [];
    }
    public class ArticleEventResponse
    {
        public Guid Id { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public ArticleEventType EventType { get; set; }
    }

    internal class Handler : IRequestHandler<Query, Result<Response>>
    {
        private readonly ApplicationDbContext _dbContext;
        public Handler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Result<Response>> Handle(Query request, CancellationToken cancellationToken)
        {
            var article = await _dbContext.Articles.FindAsync([request.Id], cancellationToken);
            if (article == null)
            {
                return Result.Failure<Response>(new Error("GetArticle.NotFound", "Article not found."));
            }

            var events = await _dbContext.ArticleEvents.Where(x => x.ArticleId == article.Id)
                .Select(x => new ArticleEventResponse
                {
                    Id = x.Id,
                    CreatedOnUtc = x.CreatedOnUtc,
                    EventType = x.EventType
                }).ToListAsync(cancellationToken);

            var response = new Response
            {
                Id = article.Id,
                CreatedOnUtc = article.CreatedOnUtc,
                PublishedOnUtc = article.PublishedOnUtc,
                Events = events
            };
            return Result.Success(response);
        }
    }
}

public class GetArticleEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/articles/{id}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetArticle.Query { Id = id });

            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }

            return Results.Ok(result.Value);
        });
    }
}