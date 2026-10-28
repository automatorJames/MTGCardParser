namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Satoru Umezawa, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Satoru Umezawa</exampledoc>
[TokenizationOrder(-1)]
public class SatoruUmezawaRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever you activate a ninjutsu ability, look at the top three cards of your library"];
}
