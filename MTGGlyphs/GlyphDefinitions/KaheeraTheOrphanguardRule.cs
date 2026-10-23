namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Kaheera, the Orphanguard, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kaheera, the Orphanguard</exampledoc>
[TokenizationOrder(-1)]
public class KaheeraTheOrphanguardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["companion — each creature card in your starting deck is a cat, elemental, nightmare, dinosaur, or beast card"];
}
