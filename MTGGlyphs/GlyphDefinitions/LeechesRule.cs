namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Leeches, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Leeches</exampledoc>
[TokenizationOrder(-1)]
public class LeechesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target player loses all poison counters"];
}
