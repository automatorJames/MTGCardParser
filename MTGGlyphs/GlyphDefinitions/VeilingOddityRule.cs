namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Veiling Oddity, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Veiling Oddity</exampledoc>
[TokenizationOrder(-1)]
public class VeilingOddityRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when the last time counter is removed from this card while it's exiled, creatures can't be blocked this turn"];
}
