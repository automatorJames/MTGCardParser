namespace MTGCardParser.TokenTesting;

/// <summary>
/// Represents a segment within a token's text. This can be either
/// a piece of plain text or a nested child token.
/// </summary>
public abstract record TokenSegment;

/// <summary>
/// Represents a segment of plain, un-tokenized text within a parent token.
/// </summary>
/// <param name="Text">The string content of the segment.</param>
/// <param name="AbsoluteStart">The absolute starting position in the source document.</param>
public record TokenSegmentLeaf(string Text, int AbsoluteStart) : TokenSegment;

/// <summary>
/// Represents a nested child token as a segment within its parent.
/// </summary>
/// <param name="ChildToken">The positional token for the child.</param>
public record TokenSegmentBranch(PositionalToken ChildToken) : TokenSegment;

