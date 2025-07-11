namespace MTGPlexer.Attributes;

[AttributeUsage(AttributeTargets.Enum)]
public class EnumOptionsAttribute : Attribute
{
    public bool WrapInWordBoundaries { get; set; } = true;
    public bool OptionalPlural { get; set; } = false;
}