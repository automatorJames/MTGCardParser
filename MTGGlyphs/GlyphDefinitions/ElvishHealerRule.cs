namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Elvish Healer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Elvish Healer</exampledoc>
[TokenizationOrder(-1)]
public class ElvishHealerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it's a green creature, prevent the next 2 damage instead"];
}
