using MTGPlexer.RegexSegmentDTOs.RegexTemplateLines;

namespace MTGPlexer.RegexSegmentDTOs;

/// <summary>
/// This record is used for strings defined in RegexTemplate expression bodies. These strings aren't associated
/// with any TokenUnit property, but rather must be matched as part of the TokenUnit's overall Regex.
/// </summary>
public class TextSegment : RegexSegmentBase
{
    public TextSegment(string pattern)
    {
        RegexString = pattern;
    }

    public override void ComposeRegexLines(List<RegexTemplateLine> lines, List<string> namePath, int indentation)
    {
        lines.Add(new TextLine(RegexString, string.Join(".", namePath), indentation));
    }
}

