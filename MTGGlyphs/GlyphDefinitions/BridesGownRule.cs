namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Bride's Gown, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Bride's Gown</exampledoc>
[TokenizationOrder(-1)]
public class BridesGownRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it gets an additional +0/+2 and has first strike as long as an equipment named groom's finery is attached to a creature you control"];
}
