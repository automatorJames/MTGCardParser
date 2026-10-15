namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A creature dealing no combat damage: "assigns no combat damage this turn" - the price of an unblocked attacker's alternative effect.</summary>
/// <exampledoc>Farrel's Zealot</exampledoc>
/// <examplecapture>assigns no combat damage this turn</examplecapture>
[Dependent]
public class AssignsNoCombatDamage : Glyph, IPredicate
{
    public override Nib[] Nibs => ["assigns no combat damage", Prop(Period)];

    public ThisPeriod Period { get; set; }
}
