namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Draco, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Draco</exampledoc>
[TokenizationOrder(-1)]
public class DracoRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this cost is reduced by {2} for each basic land type among lands you control"];
}
