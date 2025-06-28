namespace MTGCardParser.Attributes;

[AttributeUsage(AttributeTargets.Enum)]
public class RegexOptionsAttribute : Attribute
{
    public bool WrapInWordBoundaries { get; set; } = true;
    public bool OptionalPlural { get; set; } = true;
}