namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Buffs, then keywords and a quoted ability granted together: `gets +1/+0 and has flying and "whenever this creature attacks, draw a card"`, `gets +3/+3 and gains trample and "…"`. PredicatePair splits at the last " and ", which here falls between the keywords and the quote, so this pairs them explicitly. Without keywords before the quote, PredicatePair takes it.</summary>
/// <exampledoc>Take Flight</exampledoc>
/// <examplecapture>gets +1/+0 and has flying and "whenever this creature attacks, draw a card</examplecapture>
[Dependent]
public class BuffsAndQuotedAbility : Glyph, IConjoinedPredicate, IPredicate
{
    public override Joiner Joiner => Joiner.None;
    public override Nib[] Nibs => [Prop(Buffs), " and ", Prop(Verb), Prop(Alongside), " \"", Prop(Ability)];

    public OneOf<ManyOf<GainedOrLostBuff>, GainedOrLostBuff> Buffs { get; set; }
    public GrantVerb Verb { get; set; }
    public KeywordsAlongside Alongside { get; set; }
    public DynamicGlyph Ability { get; set; }
}
