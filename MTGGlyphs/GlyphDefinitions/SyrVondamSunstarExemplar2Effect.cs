namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Syr Vondam, Sunstar Exemplar, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Syr Vondam, Sunstar Exemplar</exampledoc>
[Dependent]
public class SyrVondamSunstarExemplar2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy up to one target nonland permanent"];
}
