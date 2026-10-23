namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Veiling Oddity, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Veiling Oddity</exampledoc>
[Dependent]
public class VeilingOddityEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["creatures can't be blocked this turn"];
}
