namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Damage with no amount given, as replacement and prevention effects speak of it: "damage", "combat damage", "noncombat damage" - "would deal damage to you", "would deal combat damage to a player", "would deal noncombat damage to an opponent".</summary>
/// <exampledoc>Dictate of the Twin Gods</exampledoc>
/// <examplecapture>damage</examplecapture>
[Dependent]
public class AnyDamage : Glyph
{
    public override Nib[] Nibs => [Prop(Kind), "damage"];

    [Optional]
    public DamageKind? Kind { get; set; }
}
