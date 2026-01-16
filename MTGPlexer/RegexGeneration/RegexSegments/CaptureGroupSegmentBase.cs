using MTGPlexer.CommonDTOs;
using Newtonsoft.Json.Linq;

namespace MTGPlexer.RegexGeneration.RegexSegments;

/// <summary>
/// The base class for all TokenUnit properties (or TokenUnit x-Of PolyItemCapures) with a named 
/// Regex capture group whose pattern is associated with some property, including child TokenUnit properties. 
/// Includes mechanisms for setting values for properties of all relevant types.
/// </summary>
public abstract record CaptureGroupSegmentBase : RegexSegmentBase
{
    public string Name => TemplatePropInfo.Name;
    public TemplatePropInfo TemplatePropInfo { get; init; }
    public abstract Regex ManyMatchRegex { get; }
    
    public CaptureGroupSegmentBase(TemplatePropInfo captureProp)
    {
        TemplatePropInfo = captureProp;
    }

    public bool TrySetOnParent(TokenUnit parentTokenUnit)
    {
        var scopedCaptures = parentTokenUnit.Match[Name];

        if (!scopedCaptures.Any())
            return false;

        object propertyValue = null;

        if (this is IMultiCaptureSegment multiCaptureSegment)
            propertyValue = multiCaptureSegment.GetPropertyValueFromMultiCapture(parentTokenUnit.Match, scopedCaptures);
        else
            propertyValue = GetPropertyValue(parentTokenUnit.Match, scopedCaptures.Single());

        if (propertyValue == null)
            return false;

        parentTokenUnit.SetPropertyFromCapture(TemplatePropInfo, parentTokenUnit.Match.RootMatch, propertyValue);

        return true;
    }

    public virtual object GetPropertyValue(TokenUnitMatch parentTokenUnitMatch, Capture scopedCapture)
    {
        // The default implementation returns null. This allows IMultiCaptureSegments to skip implementation
        // of this method in favor of the multi-capture version of it. It also allows special cases like
        // DynamicOfSegment instances to perform a no-op.

        return null;
    }

    public override string ToString() => base.ToString();

}