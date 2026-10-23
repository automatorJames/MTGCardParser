namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Raph & Mikey, Troublemakers, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Raph &amp; Mikey, Troublemakers</exampledoc>
[TokenizationOrder(-1)]
public class RaphMikeyTroublemakersCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever raph & mikey attack, reveal cards from the top of your library until you reveal a creature card"];
}
