namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Vizier of Remedies, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Vizier of Remedies</exampledoc>
[TokenizationOrder(-1)]
public class VizierOfRemediesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if one or more -1/-1 counters would be put on a creature you control, that many -1/-1 counters minus one are put on it instead"];
}
