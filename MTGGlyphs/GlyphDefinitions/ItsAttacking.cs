namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"it's attacking": the creature named earlier, as the attacker in <see cref="AttackedByIt"/>.</summary>
/// <exampledoc>Scorch Spitter</exampledoc>
/// <examplecapture>it's attacking</examplecapture>
[Dependent]
public class ItsAttacking : Glyph
{
    public override Nib[] Nibs => [Prop(Attacker), "'s attacking"];

    public It Attacker { get; set; }
}
