namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Wolf Strike, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Wolf Strike</exampledoc>
[TokenizationOrder(-1)]
public class WolfStrikeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target creature you control gets +2/+0 until end of turn if it's night"];
}
