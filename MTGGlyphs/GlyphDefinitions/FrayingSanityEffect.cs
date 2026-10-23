namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Fraying Sanity, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fraying Sanity</exampledoc>
/// <examplecapture>enchanted player mills x cards, where x is the number of cards put into their graveyard from anywhere this turn</examplecapture>
[Dependent]
public class FrayingSanityEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["enchanted player mills x cards, where x is the number of cards put into their graveyard from anywhere this turn"];
}
