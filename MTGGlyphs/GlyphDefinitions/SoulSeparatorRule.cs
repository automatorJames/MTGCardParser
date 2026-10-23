namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Soul Separator, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Soul Separator</exampledoc>
[TokenizationOrder(-1)]
public class SoulSeparatorRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a black zombie creature token with power equal to that card's power and toughness equal to that card's toughness"];
}
