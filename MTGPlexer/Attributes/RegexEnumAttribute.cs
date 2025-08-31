namespace MTGPlexer.Attributes;

[AttributeUsage(AttributeTargets.Enum)]
public class RegexEnumAttribute : Attribute
{
    public bool OptionalPlural { get; set; } = false;
}