namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Booby Trap, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Booby Trap</exampledoc>
[TokenizationOrder(-1)]
public class BoobyTrapRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the chosen player reveals each card they draw"];
}
