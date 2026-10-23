namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Spinerock Tyrant, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spinerock Tyrant</exampledoc>
[TokenizationOrder(-1)]
public class SpinerockTyrantCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you do, those spells gain wither"];
}
