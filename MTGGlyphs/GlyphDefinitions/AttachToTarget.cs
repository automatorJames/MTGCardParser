namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"attach to target creature you control": equip's effect, attaching the equipment itself, unnamed.</summary>
/// <exampledoc>Adventuring Gear</exampledoc>
/// <examplecapture>attach to target creature you control</examplecapture>
public class AttachToTarget : Glyph
{
    public override Nib[] Nibs => ["attach to", Prop(AttachedTo)];

    public PermanentPhrase AttachedTo { get; set; }
}
