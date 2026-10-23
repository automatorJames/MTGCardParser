namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Satoru Umezawa, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Satoru Umezawa</exampledoc>
[Dependent]
public class SatoruUmezawaEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["look at the top three cards of your library"];
}
