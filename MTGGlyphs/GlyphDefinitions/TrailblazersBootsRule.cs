namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Trailblazer's Boots, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Trailblazer's Boots</exampledoc>
[TokenizationOrder(-1)]
public class TrailblazersBootsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["equipped creature has nonbasic landwalk"];
}
