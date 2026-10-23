namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ancient Cornucopia, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ancient Cornucopia</exampledoc>
[TokenizationOrder(-1)]
public class AncientCornucopiaCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever you cast a spell that's one or more colors, you may gain 1 life for each of that spell's colors"];
}
