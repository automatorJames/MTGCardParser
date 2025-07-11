namespace MTGPlexer.TokenTesting.DTOs;

/// <summary>
/// Represents a segment within a token's text. This can be either
/// a leaf (no further branching) or a branch with a single child
/// </summary>
public abstract record TokenSegment;

