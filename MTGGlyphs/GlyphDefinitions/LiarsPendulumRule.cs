namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Liar's Pendulum, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Liar's Pendulum</exampledoc>
[TokenizationOrder(-1)]
public class LiarsPendulumRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target opponent guesses whether a card with that name is in your hand"];
}
