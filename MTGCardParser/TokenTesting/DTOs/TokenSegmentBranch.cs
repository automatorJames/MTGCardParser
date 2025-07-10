namespace MTGCardParser.TokenTesting.DTOs;

/// <summary>
/// Represents a nested child token as a segment within its parent.
/// </summary>
/// <param name="ChildToken">The positional token for the child.</param>
public record TokenSegmentBranch(PositionalToken ChildToken) : TokenSegment;