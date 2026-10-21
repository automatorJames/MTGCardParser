namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Granting a whole ability, quoted, possibly after keywords: `has "{t}: add one mana of any color"`, `gains "whenever this creature attacks, …"`, `gains trample and "{g}: regenerate this creature"`. The quoted ability is any glyph that resolves; the closing quote falls after the sentence's period.</summary>
/// <exampledoc>Abundant Growth</exampledoc>
/// <examplecapture>has "{t}: add one mana of any color</examplecapture>
[Dependent]
public class GrantsQuotedAbility : Glyph, IConjoinedPredicate, IPredicate
{
    public override Joiner Joiner => Joiner.None;
    public override Nib[] Nibs => [Prop(Verb), Prop(Alongside), " \"", Prop(Ability)];

    public GrantVerb Verb { get; set; }
    [Optional]
    public KeywordsAlongside Alongside { get; set; }
    public DynamicGlyph Ability { get; set; }
}
