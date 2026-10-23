namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Balance, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Balance</exampledoc>
/// <examplecapture>each player chooses a number of lands they control equal to the number of lands controlled by the player who controls the fewest, then sacrifices the rest</examplecapture>
[TokenizationOrder(-1)]
public class BalanceRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player chooses a number of lands they control equal to the number of lands controlled by the player who controls the fewest, then sacrifices the rest"];
}
