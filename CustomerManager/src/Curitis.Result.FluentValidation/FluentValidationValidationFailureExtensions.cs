using Curitis.Result.Errors;
using FluentValidation.Results;

namespace Curitis.Result.FluentValidation;

public static class FluentValidationValidationFailureExtensions
{
    public static ValidationError CreateValidationError(this ValidationFailure[] validationFailures) =>
       ValidationError.FromResults(
           validationFailures.Select(f =>
               Result.Failure(Error.Problem(f.ErrorCode, f.ErrorMessage))).ToArray());
}
