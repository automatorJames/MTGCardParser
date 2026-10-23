namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Transmogrant's Crown, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Transmogrant's Crown</exampledoc>
[TokenizationOrder(-1)]
public class TransmograntsCrownRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["equip {2} or {b}"];
}
