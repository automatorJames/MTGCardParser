namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"that's one or more colors", "that's red or green": a permanent narrowed by its colors after its kind.</summary>
/// <exampledoc>Ugin, the Ineffable</exampledoc>
/// <examplecapture>that's one or more colors</examplecapture>
[Dependent]
public class ThatsColored : Glyph
{
    public override Nib[] Nibs => ["that's", Prop(Colors)];

    public OneOf<ColorCount?, ManyOf<ManaColor>> Colors { get; set; }
}
