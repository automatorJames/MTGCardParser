namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Telemin Performance, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Telemin Performance</exampledoc>
[TokenizationOrder(-1)]
public class TeleminPerformanceRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that player puts all noncreature cards revealed this way into their graveyard, then you put the creature card onto the battlefield under your control"];
}
