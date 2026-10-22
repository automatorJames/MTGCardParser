namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"that creature is attacking": the creature named earlier, as the attacker in <see cref="AttackedByIt"/>.</summary>
/// <exampledoc>Raid Bombardment</exampledoc>
/// <examplecapture>that creature is attacking</examplecapture>
[Dependent]
public class ThatCreatureIsAttacking : Glyph
{
    public override Nib[] Nibs => [Prop(Attacker), "is attacking"];

    public ThatCard Attacker { get; set; }
}
