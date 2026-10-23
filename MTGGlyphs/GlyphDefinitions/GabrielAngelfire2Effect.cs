namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Gabriel Angelfire, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gabriel Angelfire</exampledoc>
[Dependent]
public class GabrielAngelfire2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose flying, first strike, trample, or rampage 3"];
}
