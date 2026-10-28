namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Kutzil, Malamet Exemplar, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kutzil, Malamet Exemplar</exampledoc>
[TokenizationOrder(-1)]
public class KutzilMalametExemplarRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever one or more creatures you control each with power greater than its base power deals combat damage to a player, draw a card"];
}
