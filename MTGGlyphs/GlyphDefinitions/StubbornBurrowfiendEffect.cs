namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Stubborn Burrowfiend, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Stubborn Burrowfiend</exampledoc>
/// <examplecapture>mill two cards, then this creature gets +x/+x until end of turn, where x is the number of creature cards in your graveyard</examplecapture>
[Dependent]
public class StubbornBurrowfiendEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["mill two cards, then this creature gets +x/+x until end of turn, where x is the number of creature cards in your graveyard"];
}
