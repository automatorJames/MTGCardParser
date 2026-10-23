namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Liar's Pendulum, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Liar's Pendulum</exampledoc>
[TokenizationOrder(-1)]
public class LiarsPendulum2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you do and your opponent guessed wrong, draw a card"];
}
