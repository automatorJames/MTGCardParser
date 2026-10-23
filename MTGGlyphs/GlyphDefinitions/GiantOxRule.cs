namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Giant Ox, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Giant Ox</exampledoc>
[TokenizationOrder(-1)]
public class GiantOxRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature crews vehicles using its toughness rather than its power"];
}
