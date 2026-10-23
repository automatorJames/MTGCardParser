namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Moment of Truth, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Moment of Truth</exampledoc>
[TokenizationOrder(-1)]
public class MomentOfTruthRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put one of those cards into your hand, one into your graveyard, and one on the bottom of your library"];
}
