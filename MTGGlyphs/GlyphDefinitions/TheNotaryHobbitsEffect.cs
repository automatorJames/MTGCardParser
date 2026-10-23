namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of The Notary Hobbits, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Notary Hobbits</exampledoc>
[Dependent]
public class TheNotaryHobbitsEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if they're not a token, create two tokens that are copies of them, except the tokens aren't legendary"];
}
