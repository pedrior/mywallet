namespace MyWallet.Shared.Errors;

public static class ErrorCombiner
{
    public static ErrorOr<Success> Combine(params IErrorOr[] results)
    {
        var errors = new List<Error>();
        foreach (var result in results)
        {
            if (result.IsError)
            {
                errors.AddRange(result.Errors!);
            }
        }

        return errors.Count is 0 ? Result.Success : errors;
    }
}