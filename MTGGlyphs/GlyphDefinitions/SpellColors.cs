namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A spell's colors, as a narrowing: "that's both red and white" (all of them), "that's white, blue, black, or red" (any of them).</summary>
/// <exampledoc>Battlegate Mimic</exampledoc>
/// <examplecapture>that's both red and white</examplecapture>
[Dependent]
public class SpellColors : Glyph
{
    public override Nib[] Nibs => ["that's", Prop(Both), Prop(Colors)];

    [Optional]
    [RegexPattern("both")]
    public bool? Both { get; set; }
    public ManyOf<ManaColor> Colors { get; set; }
}
