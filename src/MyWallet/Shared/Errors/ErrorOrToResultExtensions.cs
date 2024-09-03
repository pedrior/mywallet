using System.Diagnostics;

namespace MyWallet.Shared.Errors;

public static class ErrorOrToResultExtensions
{
    public static async Task<IResult> ToResponseAsync<T>(this Task<ErrorOr<T>> result, Func<T, IResult> response,
        HttpContext? context = null) => ToResponse(await result, response, context);

    public static IResult ToResponse<T>(this ErrorOr<T> result, Func<T, IResult> response,
        HttpContext? context = null)
    {
        return result.IsError
            ? Problem(result.Errors, context)
            : response(result.Value);
    }

    private static IResult Problem(List<Error> errors, HttpContext? context = null)
    {
        if (errors.All(e => e.Type == ErrorType.Validation))
        {
            return ValidationProblem(errors);
        }

        var error = errors[0];
        var errorsDictionary = new Dictionary<string, object>();

        if (!string.IsNullOrEmpty(error.Code))
        {
            errorsDictionary["code"] = error.Code;
        }

        errorsDictionary["description"] = error.Description;
        if (error.Metadata is not null)
        {
            errorsDictionary["metadata"] = error.Metadata;
        }

        return Results.Problem(
            statusCode: ErrorTypeToHttpStatusCode(error.Type),
            extensions: new Dictionary<string, object?>
            {
                ["trace_id"] = Activity.Current?.Id ?? context?.TraceIdentifier,
                ["errors"] = new object[] { errorsDictionary }
            });
    }

    private static IResult ValidationProblem(List<Error> errors)
    {
        return Results.ValidationProblem(
            errors: CreateValidationErrorsDictionary(errors),
            statusCode: StatusCodes.Status400BadRequest);
    }

    private static Dictionary<string, string[]> CreateValidationErrorsDictionary(IEnumerable<Error> errors)
    {
        return errors.GroupBy(e => string.IsNullOrWhiteSpace(e.Code) ? "error" : e.Code)
            .ToDictionary(g => g.Key, g => g
                .Select(e => e.Description)
                .ToArray());
    }

    private static int ErrorTypeToHttpStatusCode(ErrorType type) => type switch
    {
        ErrorType.Validation => StatusCodes.Status400BadRequest,
        ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
        ErrorType.Forbidden => StatusCodes.Status403Forbidden,
        ErrorType.NotFound => StatusCodes.Status404NotFound,
        ErrorType.Conflict => StatusCodes.Status409Conflict,
        ErrorType.Failure => StatusCodes.Status422UnprocessableEntity,
        _ => StatusCodes.Status500InternalServerError
    };
}