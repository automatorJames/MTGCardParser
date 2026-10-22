namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Damage dealt to one recipient: "4 damage to any target", "damage equal to that creature's toughness to the creature's controller", "damage to that player equal to the number of swamps they control", unmeasured, "damage to you", a bounded amount ("3 or more damage to you"), or the damage just named, "double that damage to that player", "half that damage, rounded down, to that permanent or player".</summary>
/// <exampledoc>Karma</exampledoc>
/// <examplecapture>damage to that player equal to the number of swamps they control</examplecapture>
[Dependent]
public class DamageTo : Glyph
{
    public override Nib[] Nibs => [Prop(Amount), "to", Prop(Recipient), Prop(After)];

    public OneOf<DamageAmount, MeasuredDamage, OneOf<AnyDamage, ThatDamage, OneOf<DamageThreshold, HalfThatDamage>>> Amount { get; set; }
    public Recipient Recipient { get; set; }
    [Optional]
    public EqualTo After { get; set; }
}
