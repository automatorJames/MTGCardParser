namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Supper for Spiders, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Supper for Spiders</exampledoc>
[TokenizationOrder(-1)]
public class SupperForSpidersRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put onto the battlefield under your control all creature cards in your opponents' graveyards that were put there from the battlefield this turn"];
}
