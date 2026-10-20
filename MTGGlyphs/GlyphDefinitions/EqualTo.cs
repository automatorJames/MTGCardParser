namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An amount given as "equal to" a count, a characteristic or another measure: "equal to that creature's toughness", "equal to the number of swamps they control", "equal to your devotion to white", "equal to the greatest power among creatures you control".</summary>
/// <exampledoc>Karma</exampledoc>
/// <examplecapture>equal to the number of swamps they control</examplecapture>
[Dependent]
public class EqualTo : Glyph
{
    public override Nib[] Nibs => ["equal to", Prop(Measure)];

    public Amount Measure { get; set; }
}
