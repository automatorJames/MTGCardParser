namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A player barred from casting or playing something: "can't cast creature spells", "can't cast more than one spell each turn", "can't play lands", "can't cast spells during your turn", "can't cast spells or activate abilities that aren't mana abilities".</summary>
/// <exampledoc>Grid Monitor</exampledoc>
/// <examplecapture>can't cast creature spells</examplecapture>
[Dependent]
public class CantCast : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => ["can't", Prop(Verb), Prop(What), Prop(OrActivate), Prop(Period)];

    public PlayVerb Verb { get; set; }
    public OneOf<MoreThanOneSpell, SpellsCast, PlayableCardType> What { get; set; }
    [Optional]
    public OrActivateAbilities OrActivate { get; set; }
    [Optional]
    public OneOf<ThisPeriod, DuringPhase> Period { get; set; }
}
