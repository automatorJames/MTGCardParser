namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A keyword qualifier placed before who controls the permanents: "with flying your opponents control", "without flying your opponents control", "with defender you control".</summary>
/// <exampledoc>Smog Elemental</exampledoc>
/// <examplecapture>with flying your opponents control</examplecapture>
[Dependent]
public class KeywordedControlledBy : Glyph
{
    public override Nib[] Nibs => [Prop(Keywords), Prop(Controller)];

    public WithKeywords Keywords { get; set; }
    public ControlledBy Controller { get; set; }
}
