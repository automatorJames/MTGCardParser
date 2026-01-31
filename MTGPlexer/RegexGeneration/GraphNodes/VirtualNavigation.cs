namespace MTGPlexer.RegexGeneration.GraphNodes;

public class VirtualNavigation : INavigable
{
    public string Name { get; }
    public Type Type { get; }
    public Proptions Proptions { get; }

    public VirtualNavigation(string name, Type type, Proptions proptions = Proptions.None)
    {
        Name = name;
        Type = type;
        Proptions = proptions;
    }
}
