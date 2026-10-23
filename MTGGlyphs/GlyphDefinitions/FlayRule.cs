namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Flay, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Flay</exampledoc>
[TokenizationOrder(-1)]
public class FlayRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then that player discards another card at random unless they pay {1}"];
}
