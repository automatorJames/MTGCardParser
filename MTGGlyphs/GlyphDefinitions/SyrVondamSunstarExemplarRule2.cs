namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Syr Vondam, Sunstar Exemplar, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Syr Vondam, Sunstar Exemplar</exampledoc>
[TokenizationOrder(-1)]
public class SyrVondamSunstarExemplarRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when syr vondam dies or is put into exile while its power is 4 or greater, destroy up to one target nonland permanent"];
}
