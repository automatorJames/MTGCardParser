namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Hunting Wilds, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hunting Wilds</exampledoc>
[TokenizationOrder(-1)]
public class HuntingWildsRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["they become 3/3 green creatures with haste that are still lands"];
}
