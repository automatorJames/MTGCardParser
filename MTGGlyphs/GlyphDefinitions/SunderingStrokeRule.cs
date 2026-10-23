namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sundering Stroke, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sundering Stroke</exampledoc>
[TokenizationOrder(-1)]
public class SunderingStrokeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if at least seven red mana was spent to cast this spell, instead", Nib.This, "deals 7 damage to each of those permanents and/or players"];
}
