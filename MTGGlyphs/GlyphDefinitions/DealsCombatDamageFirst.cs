namespace MTGGlyphs.GlyphDefinitions;

/// <summary>First strike's rule, as its reminder text puts it: "deal(s) combat damage before creatures without first strike".</summary>
/// <exampledoc>Black Knight</exampledoc>
/// <examplecapture>deals combat damage before creatures without first strike</examplecapture>
[Dependent]
public class DealsCombatDamageFirst : Glyph, IPredicate
{
    public override Nib[] Nibs => [Plural("deal"), "combat damage before creatures", Prop(Without)];

    public WithKeywords Without { get; set; }
}
