namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"that's one or more colors", "that's red or green", "that are white and/or blue": a permanent or spell narrowed by its colors after its kind.</summary>
/// <exampledoc>Ugin, the Ineffable</exampledoc>
/// <examplecapture>that's one or more colors</examplecapture>
[Dependent]
public class ThatsColored : Glyph
{
    public override Nib[] Nibs => [Alt("that's", "that are"), Prop(Colors)];

    public OneOf<ColorCount?, ManyOf<ManaColor>, ColorsAndOr> Colors { get; set; }
}
