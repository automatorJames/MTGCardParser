namespace MTGPlexer.Attributes;

[AttributeUsage(AttributeTargets.Enum)]
public class RegexEnumAttribute : Attribute
{
    public bool WrapInWordBoundaries { get; set; } = true;
    public bool OptionalPlural { get; set; } = false;
}