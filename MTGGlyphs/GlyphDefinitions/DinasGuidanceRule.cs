namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Dina's Guidance, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dina's Guidance</exampledoc>
[TokenizationOrder(-1)]
public class DinasGuidanceRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["search your library for a creature card, reveal it, put it into your hand or graveyard, then shuffle"];
}
