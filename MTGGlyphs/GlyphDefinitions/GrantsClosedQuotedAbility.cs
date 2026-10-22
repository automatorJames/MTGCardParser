namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Granting a quoted ability whose closing quote falls inside the sentence, before another predicate: `has "whenever this creature attacks, you gain 1 life," and is a cleric …`, `has "whenever you cast a noncreature spell, …" and is a wizard …`. The quoted ability is any glyph that resolves. (When the quote closes after the period, it's <see cref="GrantsQuotedAbility"/>; with a duration after it, <see cref="GrantsQuotedAbilityFor"/>.)</summary>
/// <exampledoc>White Mage's Staff</exampledoc>
/// <examplecapture>has "whenever this creature attacks, you gain 1 life,"</examplecapture>
[Dependent]
public class GrantsClosedQuotedAbility : Glyph, IConjoinedPredicate, IPredicate
{
    public override Joiner Joiner => Joiner.None;
    public override Nib[] Nibs => [Prop(Verb), Prop(Alongside), " \"", Prop(Ability), Alt(",\"", "\"")];

    public GrantVerb Verb { get; set; }
    [Optional]
    public KeywordsAlongside Alongside { get; set; }
    public DynamicGlyph Ability { get; set; }
}
