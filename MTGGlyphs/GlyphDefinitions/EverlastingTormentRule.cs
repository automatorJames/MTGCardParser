namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Everlasting Torment, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Everlasting Torment</exampledoc>
[TokenizationOrder(-1)]
public class EverlastingTormentRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["all damage is dealt as though its source had wither"];
}
