namespace MTGPlexer.TokenUnits;

[RegexBoundaryOptionAtrribute(BoundaryOption.None)]
public class DefaultUnmatchedString : TokenUnit
{
    public override Snippet[] Snippets => [@"[^\s]+"];

    public DefaultUnmatchedString()
    {
    }

    public DefaultUnmatchedString(string sourceText, int unmatchedStart, int unmatchedLength)
    {
        var regexForLength = new Regex($".{{{unmatchedLength}}}", RegexOptions.Singleline);
        var match = regexForLength.Match(sourceText, unmatchedStart, unmatchedLength);

        // Should always match
        if (!match.Success)
            throw new Exception();

        CaptureContext = CaptureContext.Create(match, sourceText);
    }
}