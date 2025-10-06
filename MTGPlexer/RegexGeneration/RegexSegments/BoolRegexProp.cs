
using MTGPlexer.RegexGeneration.RegexTemplateLines.Lines;

namespace MTGPlexer.RegexGeneration.RegexSegments;

/// <summary>
/// Represents a bool property on a TokenUnit. Bool property Regexes typically check for the optional presence
/// of some matching pattern. Such properties are usually expected to have a RegexPattern attribute that defines
/// its pattern(s), but in the absence of this the normalized property name is matched.
/// </summary>
public class BoolRegexProp : ScalarCapturePropBase
{
    public BoolRegexProp(RegexPropInfo captureProp) : base(captureProp)
    {
    }

    public override void ComposeRegexLines(RegexBuilder collector)
    {
        collector.OpenGroup(RegexPropInfo);
        collector.AddAlternateValues(ScalarAlternativeSet.Alternates);
        collector.CloseGroup(GroupQuantifier.Optional);
    }

    public override bool SetValueFromMatch(TokenUnit token, Match match)
    {
        var capture = match.Groups[Name];

        if (!capture.Success)
            return false;

        token.SetPropertyFromCapture(RegexPropInfo, capture, true);
        return true;
    }
}