namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Descendant of Soramaro, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Descendant of Soramaro</exampledoc>
/// <examplecapture>look at the top x cards of your library, where x is the number of cards in your hand, then put them back in any order</examplecapture>
[Dependent]
public class DescendantOfSoramaroEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["look at the top x cards of your library, where x is the number of cards in your hand, then put them back in any order"];
}
