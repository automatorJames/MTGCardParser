namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Gabriel Angelfire, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gabriel Angelfire</exampledoc>
[Dependent]
public class GabrielAngelfireEffect2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a creature with rampage 3 becomes blocked, it gets +3/+3 until end of turn for each creature blocking it beyond the first"];
}
