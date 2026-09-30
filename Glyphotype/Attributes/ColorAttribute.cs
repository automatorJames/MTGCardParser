namespace Glyphotype.Attributes;

/// <summary>A fixed display color for an enum member (see <see cref="Extensions.GetColor"/>). Glyph types don't declare colors: theirs come from <see cref="DeterministicPalette.TypePaletteSet"/>.</summary>
[AttributeUsage(AttributeTargets.Field)]
public class ColorAttribute(string hexValue) : Attribute
{
    public HexColor Color { get; set; } = new(hexValue);
}
