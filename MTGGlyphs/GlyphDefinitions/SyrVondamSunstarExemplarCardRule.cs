namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Syr Vondam, Sunstar Exemplar, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Syr Vondam, Sunstar Exemplar</exampledoc>
[TokenizationOrder(-1)]
public class SyrVondamSunstarExemplarCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever another creature you control dies or is put into exile, put a +1/+1 counter on syr vondam and you gain 1 life"];
}
