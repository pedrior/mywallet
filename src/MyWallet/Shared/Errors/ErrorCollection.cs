namespace MyWallet.Shared.Errors;

public sealed class ErrorCollection
{
    private readonly List<Error> errors = [];

    private ErrorCollection()
    {
    }

    public static ErrorCollection Empty => new();
    
    public bool HasErrors => errors.Count > 0;

    public static implicit operator ErrorOr<Success>(ErrorCollection collection) => 
        collection.HasErrors ? collection.errors : Result.Success;

    public ErrorCollection For(bool condition, Error error)
    {
        if (condition)
        {
            errors.Add(error);
        }
        
        return this;
    }
}