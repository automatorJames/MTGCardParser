namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Approach of the Second Sun, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Approach of the Second Sun</exampledoc>
[TokenizationOrder(-1)]
public class ApproachOfTheSecondSunRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if this spell was cast from your hand and you've cast another spell named", Nib.This, "this game, you win the game"];
}
