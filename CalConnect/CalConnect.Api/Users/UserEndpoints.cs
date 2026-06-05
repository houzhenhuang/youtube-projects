namespace CalConnect.Api.Users;

internal static class UserEndpoints
{
    private const string Tag = "Users";
    public const string VerifyEmail = "VerifyEmail";
    public static void Map(IEndpointRouteBuilder builder)
    {
        builder.MapPost("api/users/register", async (RegisterUser.Request request, RegisterUser registerUser) =>
        {
            var user = await registerUser.Handle(request);

            return Results.Ok(user);
        });

        builder.MapPost("api/users/login", async (LoginUser.Request request, LoginUser loginUser) =>
        {
            var user = await loginUser.Handle(request);

            return Results.Ok(user);
        });

        builder.MapGet("api/users/verify-email", async (Guid token, VerifyEmail verifyEmail) =>
        {
            bool success = await verifyEmail.Handle(token);

            return success ? Results.Ok() : Results.BadRequest("Invalid or expired token.");
        })
        .WithTags(Tag)
        .WithName(VerifyEmail);
    }
}