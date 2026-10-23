namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A player barred from activating abilities: "can't activate abilities that aren't mana abilities", "can't activate abilities of artifacts, creatures, or enchantments". Also the second half of "can't cast spells or activate abilities …" (see <see cref="CantCast"/>).</summary>
/// <exampledoc>Overwhelming Splendor</exampledoc>
/// <examplecapture>can't activate abilities that aren't mana abilities or loyalty abilities</examplecapture>
[Dependent]
public class CantActivate : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => ["can't activate", Prop(Abilities), Prop(Period)];

    public BarredAbilities Abilities { get; set; }
    [Optional]
    public OneOf<ThisPeriod, DuringPhase> Period { get; set; }
}
