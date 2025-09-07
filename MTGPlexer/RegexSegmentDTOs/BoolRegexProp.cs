using MTGPlexer.RegexSegmentDTOs.RegexTemplateLines;

namespace MTGPlexer.RegexSegmentDTOs;

/// <summary>
/// Represents a bool property on a TokenUnit. Bool property Regexes typically check for the optional presence
/// of some matching pattern. Such properties are usually expected to have a RegexPattern attribute that defines
/// its pattern(s), but in the absence of this the normalized property name is matched.
/// </summary>
public class BoolRegexProp : CaptureGroupPropBase
{
    public BoolRegexProp(RegexPropInfo captureProp) : base(captureProp)
    {
    }

    //protected override void SetRegex(RegexPropInfo captureProp)
    //{
    //    // Default implementation
    //
    //    CaptureAlternatives = (captureProp.Prop.GetCustomAttribute<RegexPatternAttribute>()?.Patterns ?? [captureProp.Name])
    //        .OrderByDescending(s => s.Length).ToList();
    //
    //    CaptureAlternativesString = string.Join('|', CaptureAlternatives);
    //    RegexString = $@"(?<{captureProp.Name}>[ ]?{CaptureAlternativesString}[ ]?)?";
    //}

    public override void ComposeRegexLines(List<RegexTemplateLine> lines, List<string> namePath, int indentation)
    {
        lines.Add(new NamedGroupOpen(RegexPropInfo.Name, string.Join('.', namePath), indentation));

        var captureAlternatives = (RegexPropInfo.Prop.GetCustomAttribute<RegexPatternAttribute>()?.Patterns ?? [RegexPropInfo.Name])
            .OrderByDescending(s => s.Length).ToList();

        bool isFirstAlternation = true;
        foreach (var alternation in captureAlternatives)
        {
            var value = new AlternateValue(alternation, string.Join('.', namePath).Dot(alternation), indentation + 1, isFirstAlternation);
            isFirstAlternation = false;
        }

        lines.Add(new GroupClose(string.Join('.', namePath), indentation));
    }

    public override bool SetValueFromMatchSpan(TokenUnit parentToken, TextSpan matchSpan)
    {
        var subMatchSpan = GetGroupSubMatch(parentToken, matchSpan);
        var valueToSet = subMatchSpan != null;
        TextSpan span = subMatchSpan ?? new TextSpan("");
        parentToken.SetPropertyCapture(RegexPropInfo, span, valueToSet);
        return true;
    }
}

