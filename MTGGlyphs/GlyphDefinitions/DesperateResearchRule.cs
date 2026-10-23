namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Desperate Research, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Desperate Research</exampledoc>
[TokenizationOrder(-1)]
public class DesperateResearchRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["reveal the top seven cards of your library and put all of them with that name into your hand"];
}
