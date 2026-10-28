namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A permanent narrowed to those attached to something: "attached to a creature", "attached to a creature or land", "attached to a creature you control", "attached to it" - auras or equipment on a permanent. Used on single targets; groups use <see cref="AttachedToReference"/>.</summary>
/// <exampledoc>Devout Harpist</exampledoc>
/// <examplecapture>attached to a creature</examplecapture>
[Dependent]
public class AttachedToObject : Glyph
{
    public override Nib[] Nibs => ["attached to", Prop(Host)];

    public OneOf<AttachmentHost, It, Them, This> Host { get; set; }
}
