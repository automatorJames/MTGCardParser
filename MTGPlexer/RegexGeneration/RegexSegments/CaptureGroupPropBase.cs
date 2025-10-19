namespace MTGPlexer.RegexGeneration.RegexSegments;

/// <summary>
/// The base class for all TokenUnit properties with a name Regex capture group whose pattern is 
/// associated with some property, including child TokenUnit properties. Includes mechanisms for 
/// setting values for properties of all relevant types.
/// </summary>
public abstract class CaptureGroupPropBase : RegexSegmentBase
{
    public string Name { get; init; }
    public RegexPropInfo RegexPropInfo { get; init; }
    public abstract Regex MatchRegex { get; }
    
    public CaptureGroupPropBase(RegexPropInfo captureProp)
    {
        Name = captureProp.Name;
        RegexPropInfo = captureProp;
    }

    public abstract bool SetValueFromMatch(TokenUnit tokenUnit, Match match);

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
    ManyOf
}

