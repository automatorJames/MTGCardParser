namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Light Up the Night, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Light Up the Night</exampledoc>
[TokenizationOrder(-1)]
public class LightUpTheNightRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it deals x plus 1 damage instead if that target is a creature or planeswalker"];
}
