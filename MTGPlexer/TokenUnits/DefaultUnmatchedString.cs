namespace MTGPlexer.TokenUnits;

[RegexBoundaryOptionAtrribute(BoundaryOption.None)]
public class DefaultUnmatchedString : TokenUnit
{
    protected override Snippet[] Snippets => [@"[^\s]+"];

    public DefaultUnmatchedString(string sourceText, int unmatchedStart, int unmatchedLength)
    {
        var regexForLength = new Regex($".{{{unmatchedLength}}}", RegexOptions.Singleline);
        var match = regexForLength.Match(sourceText, unmatchedStart, unmatchedLength);

        // Should always match
        if (!match.Success)
            throw new Exception();

        NodeGraph = new(typeof(DefaultUnmatchedString), match, sourceText);
    }
}