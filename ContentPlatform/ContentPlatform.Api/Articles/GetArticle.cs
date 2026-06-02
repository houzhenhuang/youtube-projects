using Carter;
using ContentPlatform.Api.Database;
using MediatR;
using Shared;

namespace ContentPlatform.Api.Articles;

public static class GetArticle
{
    public class Query : IRequest<Result<Response>>
    {
        public Guid Id { get; set; }
    }

    public class Response
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public List<string> Tags { get; set; } = [];
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
            var response = new Response
            {
                Id = article.Id,
                Title = article.Title,
                Content = article.Content,
                Tags = article.Tags
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