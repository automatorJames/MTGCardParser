namespace MTGCardParser.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class RegOptAttribute : Attribute
{
    public bool DoNotWrapInWordBoundaries { get; set; }
}