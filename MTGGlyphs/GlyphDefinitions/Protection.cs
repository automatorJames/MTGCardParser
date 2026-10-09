namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The protection keyword with its quality: "protection from red", "protection from artifacts".</summary>
/// <exampledoc>White Ward</exampledoc>
/// <examplecapture>protection from white</examplecapture>
[Dependent]
public class Protection : Glyph
{
    public override Nib[] Nibs => ["protection from", Prop(From)];

    public Quality From { get; set; }
}
