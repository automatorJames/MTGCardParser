namespace MTGPlexer.RegexGeneration.RegexSegments;

/// <summary>
/// The base class for all TokenUnit properties (or TokenUnit x-Of PolyItemCapures) with a named 
/// Regex capture group whose pattern is associated with some property, including child TokenUnit properties. 
/// Includes mechanisms for setting values for properties of all relevant types.
/// </summary>
public abstract record CaptureGroupSegmentBase : RegexSegmentBase
{
    public string LeafName => TemplatePropInfo.Name;
    public TemplatePropInfo TemplatePropInfo { get; init; }
    public abstract Regex ManyMatchRegex { get; }
    
    public CaptureGroupSegmentBase(TemplatePropInfo captureProp)
    {
        TemplatePropInfo = captureProp;
    }

    public ValueResult TrySetOnParent(TokenUnit parentTokenUnit, MatchTraversalState state)
    {
        //var scopedCapture = parentTokenUnit.Match[LeafName].SingleOrDefault();
        var scopedCapture = parentTokenUnit.Match.GetScopedCapture(LeafName, state);

        if (scopedCapture == null)
            return ValueResult.NamedCaptureNotFound;

        var propertyValue = GetPropertyValue(parentTokenUnit.Match, scopedCapture, out ValueResult result);

        if (result == ValueResult.Success)
            parentTokenUnit.SetPropertyFromCapture(TemplatePropInfo, scopedCapture, propertyValue);

        return result;
    }

    public abstract object GetPropertyValue(MatchTraversalState parentTokenUnitMatch, ExtractedCapture scopedCapture, out ValueResult result);


    public override string ToString() => base.ToString();

}

public enum ValueResult
{
    NamedCaptureNotFound,
    DynamicResolutionFailure,
    Failure,
    Success,
}