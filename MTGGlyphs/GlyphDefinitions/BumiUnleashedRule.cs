namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Bumi, Unleashed, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Bumi, Unleashed</exampledoc>
[TokenizationOrder(-1)]
public class BumiUnleashedRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["only land creatures can attack during that combat phase"];
}
