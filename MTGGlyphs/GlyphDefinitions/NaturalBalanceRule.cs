namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Natural Balance, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Natural Balance</exampledoc>
/// <examplecapture>each player who controls four or fewer lands may search their library for up to x basic land cards and put them onto the battlefield, where x is five minus the number of lands they control</examplecapture>
[TokenizationOrder(-1)]
public class NaturalBalanceRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player who controls four or fewer lands may search their library for up to x basic land cards and put them onto the battlefield, where x is five minus the number of lands they control"];
}
