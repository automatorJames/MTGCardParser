namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Herald of Leshrac, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Herald of Leshrac</exampledoc>
[TokenizationOrder(-1)]
public class HeraldOfLeshracRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["cumulative upkeep—gain control of a land you don't control"];
}
