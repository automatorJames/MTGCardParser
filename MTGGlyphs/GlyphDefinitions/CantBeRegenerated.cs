namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"it can't be regenerated", "they can't be regenerated": the pronoun refers to what was just destroyed.</summary>
/// <exampledoc>Crumble</exampledoc>
/// <examplecapture>it can't be regenerated</examplecapture>
public class CantBeRegenerated : Glyph
{
    public override Nib[] Nibs => [Prop(Subject), "can't be regenerated"];

    public OneOf<It, They> Subject { get; set; }
}
