namespace MTGPlexer.RegexGeneration.Graph;

public class HydratedNodeGraph : TokenUnitNode
{
    public CaptureContext CaptureContext { get; }
    public string Value => CaptureContext.FullMatch;

    public HydratedNodeGraph(Type type, Match match, string sourceText) 
        : base(parentNode: null, new TypeNavigation(type))
    {
        CaptureContext = CaptureContext.Create(match, sourceText);
    }

    public TokenUnit Hydrate()
    {
        //var instance = (TokenUnit)Activator.CreateInstance(Navigation.UnderlyingType);
        //
        //foreach (var captureChild in Children.OfType<BranchNode>())
        //{
        //    // will return false only if an underlying property has AbortIfSetPropertyToNull == true
        //    // and the property value is null
        //    var setSuccessfully = captureChild.SetPropertyValue(CaptureContext, instance);
        //
        //    if (!setSuccessfully)
        //        return null;
        //}
        //
        //instance.NodeGraph = this;
        //
        //return instance;

        return default;
    }
}