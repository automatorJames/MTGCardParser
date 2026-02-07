namespace MTGPlexer.RegexGeneration.Graph;

public class TypeNavigation : INavigable
{
    public string Name { get; }
    public Type Type { get; }
    public Proptions Proptions { get; } = Proptions.None;

    public TypeNavigation(Type type, string name = null)
    {
        Type = type;
        Name = name ?? (Nullable.GetUnderlyingType(type) ?? type).Name;
    }
}
