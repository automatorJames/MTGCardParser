namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Thassa's Intervention, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Thassa's Intervention</exampledoc>
[TokenizationOrder(-1)]
public class ThassasInterventionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["• counter target spell unless its controller pays twice {x}"];
}
