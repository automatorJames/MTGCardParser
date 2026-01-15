namespace MTGPlexer.RegexGeneration.RegexSegments;

/// <summary>
/// The base class for all TokenUnit properties with a name Regex capture group whose pattern is 
/// associated with some property, including child TokenUnit properties. Includes mechanisms for 
/// setting values for properties of all relevant types.
/// </summary>
public abstract record CaptureGroupPropBase : RegexSegmentBase
{
    public string Name => TemplatePropInfo.Name;
    public TemplatePropInfo TemplatePropInfo { get; init; }
    public abstract Regex ManyMatchRegex { get; }
    
    public CaptureGroupPropBase(TemplatePropInfo captureProp)
    {
        TemplatePropInfo = captureProp;
    }

    public bool TrySetOnParent(TokenUnit parentTokenUnit)
    {
        var namedGroup = parentTokenUnit.Match[Name];

        if (namedGroup == null)
            return false;

        var propValToSet = GetValueToSet(parentTokenUnit, namedGroup);

        // propValToSet may sometimes be null for cases like DynamicRegexProp, which will already have been handled earlier in the flow
        if (propValToSet != null)
            parentTokenUnit.SetPropertyFromCapture(TemplatePropInfo, namedGroup, propValToSet);

        return true;
    }

    public abstract object GetValueToSet(TokenUnit parentTokenUnit, Group namedGroup);

    public override string ToString() => base.ToString();

}

public enum TemplatePropType
{
    Enum,
    Placeholder,
    Dynamic,
    Bool,
    DistilledValue,
    TokenUnit,
    TokenUnitOneOf,
    ManyOf,
    ManyOfConjunction,
    CompoundOf,
    OneOf,
    OptionalOf,
}

