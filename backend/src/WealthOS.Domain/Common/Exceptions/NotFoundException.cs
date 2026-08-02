namespace WealthOS.Domain.Common.Exceptions;

public sealed class NotFoundException : DomainException
{
    public NotFoundException(string resource, object key)
        : base("not_found", $"{resource} with identifier '{key}' was not found.")
    {
        Resource = resource;
        Key = key;
    }

    public string Resource { get; }

    public object Key { get; }
}
