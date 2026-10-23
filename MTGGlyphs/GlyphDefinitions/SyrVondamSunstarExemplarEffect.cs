namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Syr Vondam, Sunstar Exemplar, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Syr Vondam, Sunstar Exemplar</exampledoc>
[Dependent]
public class SyrVondamSunstarExemplarEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a +1/+1 counter on syr vondam and you gain 1 life"];
}
