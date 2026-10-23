namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Beseech the Mirror, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Beseech the Mirror</exampledoc>
[TokenizationOrder(-1)]
public class BeseechTheMirrorRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put the exiled card into your hand if it wasn't cast this way"];
}
