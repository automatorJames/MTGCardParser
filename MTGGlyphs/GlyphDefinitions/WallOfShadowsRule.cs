namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Wall of Shadows, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Wall of Shadows</exampledoc>
[TokenizationOrder(-1)]
public class WallOfShadowsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature can't be the target of spells that can target only walls or of abilities that can target only walls"];
}
