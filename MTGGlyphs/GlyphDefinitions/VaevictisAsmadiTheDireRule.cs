namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Vaevictis Asmadi, the Dire, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Vaevictis Asmadi, the Dire</exampledoc>
[TokenizationOrder(-1)]
public class VaevictisAsmadiTheDireRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["those players sacrifice those permanents"];
}
