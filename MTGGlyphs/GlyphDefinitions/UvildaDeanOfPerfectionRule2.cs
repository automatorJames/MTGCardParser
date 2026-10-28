namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Uvilda, Dean of Perfection // Nassari, Dean of Expression, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Uvilda, Dean of Perfection // Nassari, Dean of Expression</exampledoc>
[TokenizationOrder(-1)]
public class UvildaDeanOfPerfectionRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it costs {4} less to cast this way"];
}
