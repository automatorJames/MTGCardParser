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

    public bool TrySetOnParent(TokenUnit parentTokenUnit)
    {
        var scopedCapture = parentTokenUnit.Match[LeafName].SingleOrDefault();

        if (scopedCapture == null)
            return false;

        var propertyValue = GetPropertyValue(parentTokenUnit.Match, scopedCapture);

        if (propertyValue == null)
            return false;

        parentTokenUnit.SetPropertyFromCapture(TemplatePropInfo, scopedCapture, propertyValue);

        return true;
    }

    public abstract object GetPropertyValue(MatchTraversalState parentTokenUnitMatch, ExtractedCapture scopedCapture);


    public override string ToString() => base.ToString();

}