namespace MTGPlexer.RegexGeneration.Graph;

public class TypeNavigation
{
    public string Name { get; }
    public Type Type { get; }
    public Type UnderlyingType { get; }

    public TypeNavigation(Type type, string name = null)
    {
        Type = type;
        UnderlyingType = (Nullable.GetUnderlyingType(type) ?? type);
        Name = name ?? UnderlyingType.Name;
    }
}
