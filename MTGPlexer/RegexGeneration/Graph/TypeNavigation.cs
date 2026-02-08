namespace MTGPlexer.RegexGeneration.Graph;

public class TypeNavigation
{
    public string Name { get; }
    public Type Type { get; }
    public Type UnderlyingType { get; }
    public Proptions Proptions { get; } = Proptions.None;

    public TypeNavigation(Type type, string name = null, Proptions proptions = Proptions.None)
    {
        Type = type;
        UnderlyingType = (Nullable.GetUnderlyingType(type) ?? type);
        Name = name ?? UnderlyingType.Name;
        Proptions = proptions;
    }
}
