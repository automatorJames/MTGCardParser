namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Damage dealt to one recipient: "4 damage to any target", "damage equal to that creature's toughness to the creature's controller", "damage to that player equal to the number of swamps they control".</summary>
/// <exampledoc>Karma</exampledoc>
/// <examplecapture>damage to that player equal to the number of swamps they control</examplecapture>
[Dependent]
public class DamageTo : Glyph
{
    public override Nib[] Nibs => [Prop(Amount), "to", Prop(Recipient), Prop(After)];

    public OneOf<DamageAmount, MeasuredDamage> Amount { get; set; }
    public Recipient Recipient { get; set; }
    [Optional]
    public EqualTo After { get; set; }
}
