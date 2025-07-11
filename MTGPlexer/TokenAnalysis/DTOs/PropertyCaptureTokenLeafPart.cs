namespace MTGPlexer.TokenAnalysis.DTOs;

/// <summary>
/// A part of a leaf that corresponds to a captured property on a TokenUnit.
/// </summary>
/// <param name="Text">The text of the property capture.</param>
/// <param name="Property">The metadata of the captured property.</param>
/// <param name="Position">The position of this property within the parent token's list of captures, used for consistent coloring.</param>
public record PropertyCaptureTokenLeafPart : TokenLeafPart
{
    public string Hex { get; }
    public RegexPropInfo Property { get; }
    public string Path { get; }

    public PropertyCaptureTokenLeafPart(string text, RegexPropInfo property, int index, string parentPath) : base(text)
    {
        Hex = TokenClassRegistry.PropertyCaptureColors[index % TokenClassRegistry.PropertyCaptureColors.Count];
        Property = property;
        Path = $"{parentPath}.{property.Name}";
    }

    public override string ToString() => base.ToString();
}