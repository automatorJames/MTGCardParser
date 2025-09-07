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
        _noSpaces = RegexPropInfo.BaseType.GetCustomAttribute<NoSpacesAttribute>() is not null;
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

        for (int i = 0; i < ChildSegments.Count; i++)
        {
            var segment = ChildSegments[i];
            segment.ComposeRegexLines(lines, namePath, indentation + 1);

            var shouldAddSpace =
                !_noSpaces
                && i < ChildSegments.Count - 1 // this is not the last segment
                && !(segment is BoolRegexProp) // these set their own spaces already
                && !_terminalPunctuation.Contains(segmentString.LastOrDefault()); // last char of segment isn't terminal punctation

            if (shouldAddSpace)
                regexString += " ";
        }


    }

    string WrapAndIndent(string namePath, string content)
    {
        
    }

    string Indent(string input, int depth)
    {
        const int spacesPerIndent = 4;
        return input.PadLeft(spacesPerIndent * depth);
    }

    public override string ToString() => base.ToString();
}

