using Curitis.Result;
using Curitis.Result.Errors;
using Curitis.Result.Extensions;
using CustomerManager.Api.Endpoints;
using CustomerManager.Api.Features.Users.Entities;
using FluentValidation;
using MediatR;
using OpenApiSamples.Data;

namespace CustomerManager.Api.Features.Users;

public static class GetUser
{
    internal sealed record GetUserQuery(int Id) : IRequest<Result<UserResponse>>;

    internal sealed record UserResponse(int Id, string FirstName, string LastName, string Email);

    internal class GetUserQueryValidator : AbstractValidator<GetUserQuery>
    {
        public GetUserQueryValidator()
        {
            RuleFor(c => c.Id).GreaterThanOrEqualTo(1).WithMessage("id不能小于1");
        }
    }

    internal sealed class GetUserQueryHandler : IRequestHandler<GetUserQuery, Result<UserResponse>>
    {
        private readonly AppDbContext _dbContext;

        public GetUserQueryHandler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Result<UserResponse>> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Set<User>().FindAsync(request.Id);
            if (user == null)
            {
                return Result.Failure<UserResponse>(Error.NotFound("User.NotFound", "用户不存在"));
            }

            return new UserResponse(user.Id, user.FirstName, user.LastName, user.Email);
        }
    }

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            var userRouteGroup = app.MapGroup("api/users").WithTags("用户");

            userRouteGroup.MapGet("/{id}", Handler)
            .RequireAuthorization(Infrastructure.Authorization.Enums.Permission.UserRead.ToString())
            .WithSummary("获取用户");
        }

        public static async Task<IResult> Handler(int id, ISender sender)
        {
            Result<UserResponse> result = await sender.Send(new GetUserQuery(id));

            return result.Match(value => Results.Ok(value), error => Results.BadRequest(error));
        }
    }
}