namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"attach {this} to it": an aura or equipment attaching itself to a permanent.</summary>
/// <exampledoc>Animate Dead</exampledoc>
/// <examplecapture>attach {this} to it</examplecapture>
public class Attach : Glyph
{
    public override Nib[] Nibs => ["attach", Nib.This, "to", Prop(AttachedTo)];

    public PermanentPhrase AttachedTo { get; set; }
}
