namespace MyWallet.Domain;

public abstract class EntityId(Ulid id) : ValueObject
{
    public Ulid Value { get; } = id;

    public override string ToString() => Value.ToString()!;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}