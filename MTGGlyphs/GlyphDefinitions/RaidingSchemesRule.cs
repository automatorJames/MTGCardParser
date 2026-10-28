namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Raiding Schemes, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Raiding Schemes</exampledoc>
[TokenizationOrder(-1)]
public class RaidingSchemesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each noncreature spell you cast has conspire"];
}
