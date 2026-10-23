namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Futurist Operative, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Futurist Operative</exampledoc>
[TokenizationOrder(-1)]
public class FuturistOperativeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as long as this creature is tapped, it's a human citizen with base power and toughness 1/1 and can't be blocked"];
}
