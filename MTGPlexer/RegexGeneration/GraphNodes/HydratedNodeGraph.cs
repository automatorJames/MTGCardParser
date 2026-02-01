namespace MTGPlexer.RegexGeneration.GraphNodes;

public class HydratedNodeGraph : RootNode
{
    public CaptureContext CaptureContext { get; }
    public string Value => CaptureContext.FullMatch;

    public HydratedNodeGraph(Type type, Match match, string sourceText) : base(type)
    {
        CaptureContext = CaptureContext.Create(match, sourceText);
    }

    public TokenUnit Hydrate()
    {
        var instance = (TokenUnit)Activator.CreateInstance(RootType);

        foreach (var captureChild in CaptureChildren)
        {
            // will return false only if an underlying property has AbortIfSetPropertyToNull == true
            // and the property value is null
            var setSuccessfully = captureChild.SetPropertyValue(CaptureContext, instance);

            if (!setSuccessfully)
                return null;
        }

        instance.NodeGraph = this;

        return instance;
    }
}