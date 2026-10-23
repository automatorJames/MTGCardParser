namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Spirit of Resistance, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spirit of Resistance</exampledoc>
[TokenizationOrder(-1)]
public class SpiritOfResistanceRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as long as you control a permanent of each color, prevent all damage that would be dealt to you"];
}
