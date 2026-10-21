namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Dealing an amount of damage with the recipient left as before - the replacement half of "… instead": "it deals 4 damage (instead)", "it deals double that damage (instead)".</summary>
/// <exampledoc>Burst Lightning</exampledoc>
/// <examplecapture>deals 4 damage</examplecapture>
[Dependent]
public class DealDamageAmount : Glyph, IPredicate
{
    public override Nib[] Nibs => [Pattern("deals?"), Prop(Amount)];

    public OneOf<DamageAmount, ThatDamage> Amount { get; set; }
}
