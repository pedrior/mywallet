using FluentValidation.Results;

namespace MyWallet.Shared.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(IValidator<TRequest>? validator = null)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IErrorOr<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (validator is null)
        {
            return await next();
        }

        var result = await validator.ValidateAsync(request, cancellationToken);
        return result.IsValid
            ? await next()
            : (dynamic)ConvertValidationResultToResponse(result);
    }

    private static List<Error> ConvertValidationResultToResponse(ValidationResult result) =>
        result.Errors.ConvertAll(f => Error.Validation(f.PropertyName, f.ErrorMessage));
}