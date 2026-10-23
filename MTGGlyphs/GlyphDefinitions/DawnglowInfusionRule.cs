namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Dawnglow Infusion, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dawnglow Infusion</exampledoc>
[TokenizationOrder(-1)]
public class DawnglowInfusionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you gain x life if {g} was spent to cast this spell and x life if {w} was spent to cast this spell"];
}
