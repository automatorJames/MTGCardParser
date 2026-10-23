namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Spider-Sense, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spider-Sense</exampledoc>
[TokenizationOrder(-1)]
public class SpiderSenseRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["counter target instant spell, sorcery spell, or triggered ability"];
}
