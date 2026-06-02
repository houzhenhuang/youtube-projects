using Carter;

namespace ContentPlatform.Api.Articles;

public class GetReportingArticle
{
    public sealed class Client(HttpClient httpClient)
    {
        public async Task<Response?> GetAsync(Guid id)
        {
            var response = await httpClient.GetFromJsonAsync<Response>($"api/articles/{id}");
         
            return response;
        }
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
        public int EventType { get; set; }
    }
}

public class GetReportingArticleEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/articles/{id}/reporting", async (Guid id, GetReportingArticle.Client client) =>
        {
            var article = await client.GetAsync(id);
            return article != null ? Results.Ok(article) : Results.NotFound();
        });
    }
}