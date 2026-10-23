namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Global Ruin, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Global Ruin</exampledoc>
[TokenizationOrder(-1)]
public class GlobalRuinRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player chooses from the lands they control a land of each basic land type, then sacrifices the rest"];
}
