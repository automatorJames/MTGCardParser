namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"discards the rest", "exile the rest": the unchosen cards' fate.</summary>
/// <exampledoc>Monomania</exampledoc>
/// <examplecapture>discards the rest</examplecapture>
[Dependent]
public class RestRemoved : Glyph
{
    public override Nib[] Nibs => [Prop(Fate), "the rest"];

    public RestFate Fate { get; set; }
}
