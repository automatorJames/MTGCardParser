namespace MTGPlexer.RegexGeneration.GraphNodes;

public class TypeNavigation : INavigable
{
    public string Name { get; }
    public Type Type { get; }
    public Proptions Proptions { get; } = Proptions.None;

    public TypeNavigation(Type type)
    {
        Type = type;
        Name = (Nullable.GetUnderlyingType(type) ?? type).Name;
    }
}
