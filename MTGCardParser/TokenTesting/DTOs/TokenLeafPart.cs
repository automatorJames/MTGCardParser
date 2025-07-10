namespace MTGCardParser.TokenTesting.DTOs;

/// <summary>
/// Represents a sub-segment of a TokenSegmentLeaf, ready for final rendering.
/// </summary>
/// /// <param name="Text">The raw text to render.</param>
public abstract record TokenLeafPart(string Text);