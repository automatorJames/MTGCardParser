namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"the player or planeswalker it's attacking", "the player or planeswalker that creature is attacking": whatever a creature named earlier is attacking, as a damage recipient (Hellrider, Scorch Spitter, Raid Bombardment).</summary>
/// <exampledoc>Scorch Spitter</exampledoc>
/// <examplecapture>the player or planeswalker it's attacking</examplecapture>
[Dependent]
public class AttackedByIt : Glyph
{
    public override Nib[] Nibs => ["the player or planeswalker", Prop(Attacker)];

    public OneOf<ItsAttacking, ThatCreatureIsAttacking> Attacker { get; set; }
}
