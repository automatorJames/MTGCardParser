namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"two or more creatures", "three or more non-gideon creatures": at least a number of permanents of a kind.</summary>
/// <exampledoc>Military Intelligence</exampledoc>
/// <examplecapture>two or more creatures</examplecapture>
[Dependent]
public class NOrMorePermanents : Glyph
{
    public override Nib[] Nibs => [Prop(Count), "or more", Prop(Permanents)];

    public Quantity Count { get; set; }
    public Permanents Permanents { get; set; }
}
