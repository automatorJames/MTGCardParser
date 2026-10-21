namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Power-up's reminder sentence: "reduce the cost by its mana cost if it entered this turn" (also "his"/"her" for named characters). The activation cost of a power-up ability is reduced by the permanent's mana cost if it entered this turn.</summary>
/// <exampledoc>She-Hulk</exampledoc>
/// <examplecapture>reduce the cost by her mana cost if she entered this turn</examplecapture>
public class PowerUpCostReduction : Glyph
{
    public override Nib[] Nibs => ["reduce the cost by", Pattern("(its|his|her)"), "mana cost if", Pattern("(it|he|she)"), "entered this turn"];
}
