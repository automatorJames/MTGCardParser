namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Zirda, the Dawnwaker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Zirda, the Dawnwaker</exampledoc>
[TokenizationOrder(-1)]
public class ZirdaTheDawnwakerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["companion — each permanent card in your starting deck has an activated ability"];
}
