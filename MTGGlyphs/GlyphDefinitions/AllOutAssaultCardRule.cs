namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of All-Out Assault, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>All-Out Assault</exampledoc>
[TokenizationOrder(-1)]
public class AllOutAssaultCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when you next attack this turn, untap each creature you control"];
}
