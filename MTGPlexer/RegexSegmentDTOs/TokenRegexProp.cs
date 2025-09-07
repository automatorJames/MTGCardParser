using MTGPlexer.RegexSegmentDTOs.RegexTemplateLines;

namespace MTGPlexer.RegexSegmentDTOs;

/// <summary>
/// Represents a property on a TokenUnit whose property type is also a TokenUnit (i.e. a child TokenUnit). During
/// compilation of a RegexTemplate, thie record simply creates an instance of the child TokenUnit type and gets its
/// rendered Regex string to add it to the parent TokenUnit's own rendered Regex.
/// </summary>
public class TokenRegexProp : CaptureGroupPropBase
{
    static HashSet<char> _terminalPunctuation = ['.', ',', ';'];
    bool _noSpaces;

    public List<RegexSegmentBase> ChildSegments { get; set; }

    public TokenRegexProp(RegexPropInfo captureProp) : base(captureProp)
    {
        var template = TokenTypeRegistry.GetTypeTemplate(captureProp.UnderlyingType);
        ChildSegments = template.RegexSegments;
    }

    protected override void SetRegex(RegexPropInfo captureProp)
    {
        var template = TokenTypeRegistry.GetTypeTemplate(captureProp.UnderlyingType);
        RegexString = template.RegexStringNoWordBoundaries;
    }

    public override void ComposeRegexLines(List<RegexTemplateLine> lines = null, List<string> namePath = null, int indentation = 0)
    {
        namePath ??= [];
        namePath.Add(RegexPropInfo.Name);
        lines ??= [];
        lines.Add(new NamedGroupOpen(RegexPropInfo.Name, string.Join('.', namePath), indentation));

        foreach (var segment in ChildSegments)
            segment.ComposeRegexLines(lines, namePath, indentation + 1);
    }

    public override string ToString() => base.ToString();
}

