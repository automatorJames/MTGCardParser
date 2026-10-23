namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Archon of the Wild Rose, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Archon of the Wild Rose</exampledoc>
[TokenizationOrder(-1)]
public class ArchonOfTheWildRoseRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["other creatures you control that are enchanted by auras you control have base power and toughness 4/4 and have flying"];
}
