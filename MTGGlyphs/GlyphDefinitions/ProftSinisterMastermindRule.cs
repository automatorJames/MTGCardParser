namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Proft, Sinister Mastermind, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Proft, Sinister Mastermind</exampledoc>
[TokenizationOrder(-1)]
public class ProftSinisterMastermindRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["threshold — you can't cast this spell unless there are seven or more cards in your graveyard"];
}
