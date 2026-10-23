namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Information Dealer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Information Dealer</exampledoc>
/// <examplecapture>look at the top x cards of your library, where x is the number of wizards on the battlefield, then put them back in any order</examplecapture>
[Dependent]
public class InformationDealerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["look at the top x cards of your library, where x is the number of wizards on the battlefield, then put them back in any order"];
}
