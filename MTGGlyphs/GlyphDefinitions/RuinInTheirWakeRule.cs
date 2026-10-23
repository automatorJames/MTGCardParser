namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ruin in Their Wake, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ruin in Their Wake</exampledoc>
[TokenizationOrder(-1)]
public class RuinInTheirWakeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may put that card onto the battlefield tapped if you control a land named wastes"];
}
