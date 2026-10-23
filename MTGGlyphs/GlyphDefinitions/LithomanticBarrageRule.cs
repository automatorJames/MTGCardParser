namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Lithomantic Barrage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lithomantic Barrage</exampledoc>
[TokenizationOrder(-1)]
public class LithomanticBarrageRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it deals 5 damage instead if that target is white and/or blue"];
}
