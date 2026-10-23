namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of No Witnesses, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>No Witnesses</exampledoc>
[TokenizationOrder(-1)]
public class NoWitnessesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player who controls the most creatures investigates"];
}
