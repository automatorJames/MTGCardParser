namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of The Chief Warg, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Chief Warg</exampledoc>
[TokenizationOrder(-1)]
public class TheChiefWargRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["ferocious — whenever you attack while you control a creature with power 4 or greater, you draw a card and lose 1 life"];
}
