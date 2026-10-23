namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Island Sanctuary, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Island Sanctuary</exampledoc>
[TokenizationOrder(-1)]
public class IslandSanctuaryRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you do, until your next turn, you can't be attacked except by creatures with flying and/or islandwalk"];
}
