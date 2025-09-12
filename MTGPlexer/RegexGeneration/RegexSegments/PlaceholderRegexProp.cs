using MTGPlexer.CommonDTOs.StructuredMatches;

namespace MTGPlexer.RegexGeneration.RegexSegments;

/// <summary>
/// Represents a placeholder text property of type PlaceholderCapture. This property type will typically have
/// a RegexPattern attribute defining its pattern, but in the absence of one the normalized property name will
/// be used as a pattern instead. This record is tightly coupled with the PlaceholderCapture type, which represents
/// a capture that's a placeholder in the sense that the caller wants to capture the given pattern but doesn't 
/// know how to decompose it yet, or the containing TokenUnit overrides SetPropertiesFromMatch and needs a property
/// to store an interim text value.
/// </summary>
public class PlaceholderRegexProp : ScalarCapturePropBase
{
    public PlaceholderRegexProp(RegexPropInfo captureProp) : base(captureProp)
    {
    }

    public override void ComposeRegexLines(RegexLineCollector collector)
    {
        collector.OpenGroup(RegexPropInfo);
        collector.AddAlternatiingValues(ScalarAlternativeSet.Alternatives);
        collector.CloseGroup();
    }

    public override bool SetValueFromMatch(TokenUnit token, StructuredMatchBase parentMatch)
    {
        var childMatch = parentMatch.GetChildMatch(this);
        var valueToSet = new PlaceholderCapture(childMatch.Value);
        token.SetPropertyFromMatch(RegexPropInfo, childMatch, valueToSet);
        return true;
    }
}

