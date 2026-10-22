namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Granting a quoted ability for a time, the duration after the closing quote: `gains "{t}: draw a card" for as long as you control this creature`, `gain "{t}: this creature deals 2 damage to any target" until end of turn`. Separate from <see cref="GrantsQuotedAbility"/>, whose quote closes after the period, so the duration here is required.</summary>
/// <exampledoc>Quicksmith Spy</exampledoc>
/// <examplecapture>gains "{t}: draw a card" for as long as you control this creature</examplecapture>
[Dependent]
public class GrantsQuotedAbilityFor : Glyph, IConjoinedPredicate, IPredicate
{
    public override Joiner Joiner => Joiner.None;
    public override Nib[] Nibs => [Prop(Verb), Prop(Alongside), " \"", Prop(Ability), "\" ", Prop(Duration)];

    public GrantVerb Verb { get; set; }
    [Optional]
    public KeywordsAlongside Alongside { get; set; }
    public DynamicGlyph Ability { get; set; }
    public EffectDuration Duration { get; set; }
}
