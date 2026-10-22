namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Permanents narrowed to those attached to something named earlier or targeted: "attached to it", "attached to them", "attached to {this}", "attached to that creature", "attached to target land" - "all auras attached to it", "the number of curses attached to them", "all equipment attached to that creature".</summary>
/// <exampledoc>Hubris</exampledoc>
/// <examplecapture>attached to it</examplecapture>
[Dependent]
public class AttachedToReference : Glyph
{
    public override Nib[] Nibs => ["attached to", Prop(Host)];

    public OneOf<OneOf<It, Them, This>, ThatCard, AttachmentTarget> Host { get; set; }
}
