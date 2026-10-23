namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Two exceptions to a copy's values, joined by "and": "except it's not legendary and it's a 4/4 hero in addition to its other types", "except it's 1/1 and it's a nightmare in addition to its other types".</summary>
/// <exampledoc>Nightmare Shepherd</exampledoc>
/// <examplecapture>except it's 1/1 and it's a nightmare in addition to its other types</examplecapture>
[Dependent]
public class CopyChangePair : Glyph
{
    public override Nib[] Nibs => [Prop(First), Prop(Second)];

    public OneOf<CopyLegendary, CopyIsDescribed> First { get; set; }
    public CopyIsDescribed Second { get; set; }
}
