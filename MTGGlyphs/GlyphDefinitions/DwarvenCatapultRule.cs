namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Dwarven Catapult, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dwarven Catapult</exampledoc>
[TokenizationOrder(-1)]
public class DwarvenCatapultRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "deals x damage divided evenly, rounded down, among all creatures target opponent controls"];
}
