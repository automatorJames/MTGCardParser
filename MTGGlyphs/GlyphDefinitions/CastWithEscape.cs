namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"it escaped": the permanent was cast from a graveyard with its escape ability.</summary>
/// <exampledoc>Uro, Titan of Nature's Wrath</exampledoc>
/// <examplecapture>it escaped</examplecapture>
[Dependent]
public class CastWithEscape : Glyph
{
    public override Nib[] Nibs => [Prop(Permanent), "escaped"];

    public OneOf<It, This> Permanent { get; set; }
}
