namespace MyWallet.Shared.Errors;

public static class ErrorOrExtensions
{
    public static ErrorOr<TValue> ThenDoOrFail<TValue>(
        this ErrorOr<TValue> result,
        Func<TValue, IErrorOr> action)
    {
        if (result.IsError)
        {
            return (ErrorOr<TValue>)result.Errors;
        }

        var actionResult = action(result.Value);
        return actionResult.IsError
            ? (ErrorOr<TValue>)actionResult.Errors!
            : result;
    }

    public static async Task<ErrorOr<TValue>> ThenDoOrFailAsync<TValue>(
        this ErrorOr<TValue> result,
        Func<TValue, Task<IErrorOr>> action)
    {
        if (result.IsError)
        {
            return (ErrorOr<TValue>)result.Errors;
        }

        var actionResult = await action(result.Value);
        return actionResult.IsError
            ? (ErrorOr<TValue>)actionResult.Errors!
            : result;
    }

    public static async Task<ErrorOr<TValue>> ThenDoOrFail<TValue>(
        this Task<ErrorOr<TValue>> result,
        Func<TValue, IErrorOr> action)
    {
        return (await result).ThenDoOrFail(action);
    }
    
    public static async Task<ErrorOr<TValue>> ThenDoOrFailAsync<TValue>(
        this Task<ErrorOr<TValue>> result,
        Func<TValue, Task<IErrorOr>> action)
    {
        return await (await result).ThenDoOrFailAsync(action);
    }
}