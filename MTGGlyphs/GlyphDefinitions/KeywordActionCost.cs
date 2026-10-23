namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A keyword action paid as a cost, with its own mana payment where it takes one: "waterbend {5}", "forage". Offered last in <see cref="Cost"/>.</summary>
/// <exampledoc>Katara, Bending Prodigy</exampledoc>
/// <examplecapture>waterbend {6}</examplecapture>
[Dependent]
public class KeywordActionCost : Glyph
{
    public Keyword Action { get; set; }
    [Optional]
    public ManaCost Payment { get; set; }
}
