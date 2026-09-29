namespace Glyphotype.GlyphPrimitives;

[RegexBoundaryOptionAtrribute(BoundaryOption.None)]
public class UnmatchedString : CaptureUnit
{
    public UnmatchedString()
    {
    }

    static readonly UnmatchedGlyphNode _rootNode = CreateSharedRootNode(new UnmatchedGlyphNode(null, new(typeof(UnmatchedString))));

    public UnmatchedString(string sourceText, int unmatchedStart, int unmatchedLength)
    {
        InitializeSpanContext(_rootNode, sourceText, unmatchedStart, unmatchedLength);
    }
}