namespace MTGCardParser.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum)]
public class RegOptAttribute : Attribute
{
    public bool DoNotWrapInWordBoundaries { get; set; }
    public bool OptionalPlural { get; set; }
}