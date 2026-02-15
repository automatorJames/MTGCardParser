namespace MTGPlexer.RegexGeneration.Graph;

public class TypeNavigation
{
    public string Name { get; }
    public Type Type { get; }
    public Type UnderlyingType { get; }
    public string[] Patterns { get; }

    public TypeNavigation(Type type, string name = null, string[] patterns = null)
    {
        Type = type;
        UnderlyingType = (Nullable.GetUnderlyingType(type) ?? type);
        Name = name ?? UnderlyingType.Name;
        Patterns = patterns ?? Type.GetCustomAttribute<RegexPatternAttribute>()?.Patterns;
    }
}
