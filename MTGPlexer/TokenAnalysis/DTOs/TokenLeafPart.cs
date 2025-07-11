namespace MTGPlexer.TokenAnalysis.DTOs;

/// <summary>
/// Represents a sub-segment of a TokenSegmentLeaf, ready for final rendering.
/// </summary>
public abstract record TokenLeafPart(string Text)
{
    public override string ToString() => Text;
}