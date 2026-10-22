namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"sources with the chosen name", "sources with that name": damage sources or permanents picked out by a card name named earlier.</summary>
/// <exampledoc>Pithing Needle</exampledoc>
/// <examplecapture>sources with the chosen name</examplecapture>
[Dependent]
public class NamedSources : Glyph
{
    public override Nib[] Nibs => ["sources", Prop(Name)];

    public WithName Name { get; set; }
}
