namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"controls a white creature", "controls an artifact or enchantment": a player controlling a permanent of a kind.</summary>
/// <exampledoc>Disorder</exampledoc>
/// <examplecapture>controls a white creature</examplecapture>
[Dependent]
public class ControlsAPermanent : Glyph
{
    public override Nib[] Nibs => ["controls", Prop(Permanent)];

    public IndefinitePermanent Permanent { get; set; }
}
