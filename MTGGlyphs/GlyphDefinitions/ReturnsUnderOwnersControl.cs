namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Reminder that a permanent exiled until something leaves the battlefield comes back to its owner: "that creature returns under its owner's control".</summary>
/// <exampledoc>Silkwrap</exampledoc>
/// <examplecapture>that creature returns under its owner's control</examplecapture>
public class ReturnsUnderOwnersControl : Glyph
{
    public override Nib[] Nibs => [Prop(Exiled), "returns under its owner's control"];

    public PermanentPhrase Exiled { get; set; }
}
