namespace MTGCardParser;

/// <summary>
/// A discriminated union that can hold either a single string or a string array.
/// Uses implicit conversions to allow for clean construction syntax.
/// </summary>
public readonly struct TemplateSnippet
{
    private readonly string _singleValue;
    private readonly string[] _arrayValue;
    public bool IsSingle => _singleValue != null;

    // Private constructors force the use of the clean implicit operators
    private TemplateSnippet(string value) => _singleValue = value;
    private TemplateSnippet(string[] value) => _arrayValue = value;

    // The magic: implicit conversions from the types you want to use
    public static implicit operator TemplateSnippet(string value) => new(value);
    public static implicit operator TemplateSnippet(string[] value) => new(value);

    // A clean way to process the value without exposing the raw fields
    public void Process(Action<string> onSingle, Action<string[]> onArray)
    {
        if (IsSingle)
            onSingle(_singleValue);
        else
            onArray(_arrayValue);
    }   
}
