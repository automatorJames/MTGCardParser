namespace MTGPlexer.TokenAnalysisDTOs.GraphNodes;

public record HydratedNodeGraph : RootNode
{
    public CaptureDictionary CaptureDictionary { get; }
    public string Value => CaptureDictionary.Value;

    public HydratedNodeGraph(Type type, Match match, string sourceText) : base(type)
    {
        CaptureDictionary = new(match, sourceText);
    }

    public TokenUnit Hydrate()
    {
        var instance = (TokenUnit)Activator.CreateInstance(RootType);

        foreach (var captureChild in CaptureChildren)
            captureChild.SetPropertyValue(CaptureDictionary, instance);

        instance.NodeGraph = this;

        return instance;
    }
}