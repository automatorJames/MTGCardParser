namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Freyalise's Winds, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Freyalise's Winds</exampledoc>
[TokenizationOrder(-1)]
public class FreyalisesWindsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a permanent with a wind counter on it would untap during its controller's untap step, remove all wind counters from it instead"];
}
