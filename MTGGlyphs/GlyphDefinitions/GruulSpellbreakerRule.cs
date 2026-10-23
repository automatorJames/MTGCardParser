namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Gruul Spellbreaker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gruul Spellbreaker</exampledoc>
[TokenizationOrder(-1)]
public class GruulSpellbreakerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["during your turn, you and this creature have hexproof"];
}
