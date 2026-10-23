namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Hand of Vecna, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hand of Vecna</exampledoc>
/// <examplecapture>equipped creature or a creature you control named vecna gets +x/+x until end of turn, where x is the number of cards in your hand</examplecapture>
[Dependent]
public class HandOfVecnaEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["equipped creature or a creature you control named vecna gets +x/+x until end of turn, where x is the number of cards in your hand"];
}
