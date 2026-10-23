namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Bludgeon Brawl, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Bludgeon Brawl</exampledoc>
[TokenizationOrder(-1)]
public class BludgeonBrawlRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each noncreature, non-equipment artifact is an equipment with equip {x} and \"equipped creature gets +x/+0,\" where x is that artifact's mana value"];
}
