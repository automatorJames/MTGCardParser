namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Path of Peril, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Path of Peril</exampledoc>
[TokenizationOrder(-1)]
public class PathOfPerilRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy all creatures [with mana value 2 or less]"];
}
