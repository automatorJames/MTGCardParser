namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Great Divide Guide, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Great Divide Guide</exampledoc>
[TokenizationOrder(-1)]
public class GreatDivideGuideRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each land and ally you control has \"{t}: add one mana of any color"];
}
