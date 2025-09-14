namespace Magnett.Automation.Core.Contexts;

/// <summary>
/// Class representing a field in a context, specifically for a value of type <typeparamref name="TValue"/>.
/// </summary>
/// /// <example>
/// var field = ContextField/<string/>.WithName("MyField");
/// </example>
/// <typeparam name="TValue"></typeparam>
public record ContextField<TValue> : CommonNamedKey
{
    private ContextField(string name) : base(name)
    {

    }
    
    public static implicit operator ContextField<TValue>(string name)
    {
        return WithName(name);       
    }

    public static ContextField<TValue> WithName(string fieldName)
    {
        return new ContextField<TValue>(fieldName);
    }
}