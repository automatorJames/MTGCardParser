namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"that convoked it": the creatures tapped to pay for this spell with convoke, as a qualifier on a group of permanents (Venerated Loxodon, Zephyr Singer, Ancient Imperiosaur).</summary>
/// <exampledoc>Venerated Loxodon</exampledoc>
/// <examplecapture>that convoked it</examplecapture>
[Dependent]
public class ThatConvokedIt : Glyph
{
    public override Nib[] Nibs => ["that convoked", Prop(Spell)];

    public It Spell { get; set; }
}
