namespace MTGPlexer.RegexGeneration.RegexSegments;

/// <summary>
/// The base class for all TokenUnit properties with a name Regex capture group whose pattern is 
/// associated with some property, including child TokenUnit properties. Includes mechanisms for 
/// setting values for properties of all relevant types.
/// </summary>
public abstract record CaptureGroupPropBase : RegexSegmentBase
{
    public string Name => RegexPropInfo.Name;
    public RegexPropInfo RegexPropInfo { get; init; }
    public abstract Regex ManyMatchRegex { get; }
    
    public CaptureGroupPropBase(RegexPropInfo captureProp)
    {
        RegexPropInfo = captureProp;
    }

    public abstract bool SetValueFromNamedGroupInMatch(TokenUnit tokenUnit);

    public override string ToString() => base.ToString();

}

public enum RegexPropType
{
    Enum,
    Placeholder,
    Dynamic,
    Bool,
    DistilledValue,
    TokenUnit,
    TokenUnitOneOf,
    ManyOf,
    ManyOfItem,
    ManyOfConjunction,
}

