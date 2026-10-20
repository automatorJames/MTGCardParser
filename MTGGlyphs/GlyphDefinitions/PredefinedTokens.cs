namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Predefined tokens named by their artifact subtype, whose characteristics the rules supply: "a treasure token", "two food tokens", "a tapped powerstone token".</summary>
/// <exampledoc>Brazen Freebooter</exampledoc>
/// <examplecapture>a treasure token</examplecapture>
[Dependent]
public class PredefinedTokens : Glyph
{
    public override Nib[] Nibs => [Prop(Quantity), Prop(Tapped), Prop(Kind), Plural("token")];

    public Quantity Quantity { get; set; }
    [RegexPattern("tapped")]
    public bool Tapped { get; set; }
    public PredefinedToken Kind { get; set; }
}
