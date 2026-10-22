namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Life paid in an amount measured by something: "life equal to its toughness", "life equal to its mana value".</summary>
/// <exampledoc>Essence Vortex</exampledoc>
/// <examplecapture>life equal to its toughness</examplecapture>
[Dependent]
public class LifeEqualTo : Glyph
{
    public override Nib[] Nibs => ["life", Prop(Amount)];

    public EqualTo Amount { get; set; }
}
