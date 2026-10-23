namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"or more", "or less": the open end of a threshold.</summary>
/// <exampledoc>Martial Coup</exampledoc>
/// <examplecapture>or more</examplecapture>
[Dependent]
public class OrBound : Glyph
{
    public override Nib[] Nibs => ["or", Prop(Bound)];

    public CountBound Bound { get; set; }
}
