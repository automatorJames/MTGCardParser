namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Stubborn Burrowfiend, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Stubborn Burrowfiend</exampledoc>
[TokenizationOrder(-1)]
public class StubbornBurrowfiendCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever this creature becomes saddled for the first time each turn, mill two cards, then this creature gets +x/+x until end of turn, where x is the number of creature cards in your graveyard"];
}
