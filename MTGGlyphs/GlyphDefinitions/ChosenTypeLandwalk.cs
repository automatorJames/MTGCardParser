namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Landwalk of a land type named earlier: "landwalk of the chosen type", "snow landwalk of the chosen type", "landwalk of that type" - a keyword ability whose land type was chosen.</summary>
/// <exampledoc>Traveler's Cloak</exampledoc>
/// <examplecapture>landwalk of the chosen type</examplecapture>
[Dependent]
public class ChosenTypeLandwalk : Glyph
{
    public override Nib[] Nibs => [Prop(Snow), "landwalk", Prop(OfType)];

    [RegexPattern("snow")]
    public bool Snow { get; set; }
    public NamedTypeReference OfType { get; set; }
}
