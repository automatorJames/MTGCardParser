namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Soul Burn, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Soul Burn</exampledoc>
[TokenizationOrder(-1)]
public class SoulBurnRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["spend only black and/or red mana on x"];
}
