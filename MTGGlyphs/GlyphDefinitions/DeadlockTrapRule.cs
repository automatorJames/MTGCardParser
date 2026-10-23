namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Deadlock Trap, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Deadlock Trap</exampledoc>
[TokenizationOrder(-1)]
public class DeadlockTrapRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["its activated abilities can't be activated this turn"];
}
