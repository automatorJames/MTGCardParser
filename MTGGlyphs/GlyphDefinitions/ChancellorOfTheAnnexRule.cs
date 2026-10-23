namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Chancellor of the Annex, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Chancellor of the Annex</exampledoc>
[TokenizationOrder(-1)]
public class ChancellorOfTheAnnexRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you do, when each opponent casts their first spell of the game, counter that spell unless that player pays {1}"];
}
