namespace MTGPlexer.TokenTesting.DTOs;

/// <summary>
/// A part of a leaf that corresponds to a captured property on a TokenUnit.
/// </summary>
/// <param name="Text">The text of the property capture.</param>
/// <param name="Property">The metadata of the captured property.</param>
/// <param name="PropertyIndex">The original index of this property within the parent token's list of captures, used for consistent coloring.</param>
public record PropertyCaptureTokenLeafPart(string Text, RegexPropInfo Property, int PropertyIndex) : TokenLeafPart(Text);