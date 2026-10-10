namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Dealing damage, to one recipient or several: "deals 1 damage to that player", "deals 4 damage to any target and 2 damage to you".</summary>
/// <exampledoc>Psionic Blast</exampledoc>
/// <examplecapture>deals 4 damage to any target and 2 damage to you</examplecapture>
[Dependent]
public class DealDamage : Glyph, IPredicate
{
    public override Nib[] Nibs => [Pattern("deals?"), Prop(Portions)];

    public OneOf<ManyOf<DamageTo>, DamageTo> Portions { get; set; }
}
