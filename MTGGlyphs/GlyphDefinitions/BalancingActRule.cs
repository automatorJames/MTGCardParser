namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Balancing Act, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Balancing Act</exampledoc>
/// <examplecapture>each player chooses a number of permanents they control equal to the number of permanents controlled by the player who controls the fewest, then sacrifices the rest</examplecapture>
[TokenizationOrder(-1)]
public class BalancingActRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player chooses a number of permanents they control equal to the number of permanents controlled by the player who controls the fewest, then sacrifices the rest"];
}
