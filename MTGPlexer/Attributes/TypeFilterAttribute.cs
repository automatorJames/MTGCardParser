namespace MTGPlexer.Attributes;

public class TypeFilterAttribute(Type type) : Attribute
{
    public Type Type { get; set; } = type;
}
