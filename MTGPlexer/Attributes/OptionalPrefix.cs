namespace MTGPlexer.Attributes;

[AttributeUsage(AttributeTargets.Enum)]
public class OptionalPrefix(string prefixSnippet) : Attribute
{
    public string PrefixSnippet { get; set; } = prefixSnippet;
}