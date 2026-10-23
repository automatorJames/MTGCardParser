namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Endless Detour, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Endless Detour</exampledoc>
/// <examplecapture>the owner of target spell, nonland permanent, or card in a graveyard puts it on their choice of the top or bottom of their library</examplecapture>
[TokenizationOrder(-1)]
public class EndlessDetourRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the owner of target spell, nonland permanent, or card in a graveyard puts it on their choice of the top or bottom of their library"];
}
