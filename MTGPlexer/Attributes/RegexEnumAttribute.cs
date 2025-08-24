namespace MTGPlexer.Attributes;

[AttributeUsage(AttributeTargets.Enum)]
public class RegexEnumAttribute : Attribute
{
    public bool WrapInWordBoundaries { get; set; } = false;
    public bool OptionalPlural { get; set; } = false;
}