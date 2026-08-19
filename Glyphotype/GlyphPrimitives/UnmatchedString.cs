namespace Glyphotype.GlyphPrimitives;

[RegexBoundaryOptionAtrribute(BoundaryOption.None)]
public class UnmatchedString : CaptureUnit
{
    public UnmatchedString()
    {
    }

    public UnmatchedString(string sourceText, int unmatchedStart, int unmatchedLength)
    {
        var regexForLength = new Regex($".{{{unmatchedLength}}}", RegexOptions.Singleline);
        var match = regexForLength.Match(sourceText, unmatchedStart, unmatchedLength);

        // Should always match
        if (!match.Success)
            throw new Exception();

        CaptureContext = new(new UnmatchedGlyphNode(null, new(typeof(UnmatchedString))), match, sourceText);
    }
}