namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Kutzil, Malamet Exemplar, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kutzil, Malamet Exemplar</exampledoc>
[Dependent]
public class KutzilMalametExemplarEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["draw a card"];
}
