namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Zareth San, the Trickster, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Zareth San, the Trickster</exampledoc>
[Dependent]
public class ZarethSanTheTricksterEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put this card from your hand onto the battlefield tapped and attacking"];
}
