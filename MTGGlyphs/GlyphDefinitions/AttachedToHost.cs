namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"attached to that creature", "attached to target player", "attached to this creature": what an aura, equipment or curse put onto the battlefield is attached to.</summary>
/// <exampledoc>Dragon Breath</exampledoc>
/// <examplecapture>attached to that creature</examplecapture>
[Dependent]
public class AttachedToHost : Glyph
{
    public override Nib[] Nibs => ["attached to", Prop(Host)];

    public Recipient Host { get; set; }
}
