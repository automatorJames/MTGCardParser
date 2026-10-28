namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Balduvian Shaman, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Balduvian Shaman</exampledoc>
[TokenizationOrder(-1)]
public class BalduvianShamanRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["(for example, you may change \"black creatures can't attack\" to \"blue creatures can't attack"];
}
