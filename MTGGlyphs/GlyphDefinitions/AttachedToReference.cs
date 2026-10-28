namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Permanents narrowed to those attached to something named earlier, targeted, or of a kind: "attached to it", "attached to them", "attached to {this}", "attached to that creature", "attached to target land", "that's attached to a creature" - "all auras attached to it", "the number of curses attached to them", "each aura you control that's attached to a creature".</summary>
/// <exampledoc>Hubris</exampledoc>
/// <examplecapture>attached to it</examplecapture>
[Dependent]
public class AttachedToReference : Glyph
{
    public override Nib[] Nibs => [Opt(Alt("that's", "that are")), "attached to", Prop(Host)];

    public OneOf<It, Them, This, ThatCard, AttachmentTarget, AttachmentHostKind> Host { get; set; }
}
