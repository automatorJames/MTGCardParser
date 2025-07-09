namespace MTGCardParser.TokenTesting;

/// <summary>
/// Represents a sub-segment of a TokenSegmentLeaf, ready for final rendering.
/// </summary>
public abstract record LeafPart;

/// <summary>
/// A part of a leaf that is simple, un-captured text.
/// </summary>
/// <param name="Text">The raw text to render.</param>
public record PlainTextPart(string Text) : LeafPart;

/// <summary>
/// A part of a leaf that corresponds to a captured token property.
/// </summary>
/// <param name="Text">The text of the property capture.</param>
/// <param name="Property">The metadata of the captured property.</param>
/// <param name="PropertyIndex">The original index of this property within the parent token's list of captures, used for consistent coloring.</param>
public record PropertyCapturePart(string Text, RegexPropInfo Property, int PropertyIndex) : LeafPart;