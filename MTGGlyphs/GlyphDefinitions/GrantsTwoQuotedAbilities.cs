namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Granting two quoted abilities at once: `has "{t}: add {c}" and "{t}, pay 1 life: add one mana of any color"`, `gains "…" and "…"`. The second quote closes after the sentence's period.</summary>
/// <exampledoc>Thornbite Staff</exampledoc>
/// <examplecapture>has "{2}, {t}: this creature deals 1 damage to any target" and "whenever a creature dies, untap this creature</examplecapture>
[Dependent]
public class GrantsTwoQuotedAbilities : Glyph, IConjoinedPredicate, IPredicate
{
    public override Joiner Joiner => Joiner.None;
    public override Nib[] Nibs => [Prop(Verb), " \"", Prop(First), "\" and \"", Prop(Second)];

    public GrantVerb Verb { get; set; }
    public DynamicGlyph First { get; set; }
    public DynamicGlyph Second { get; set; }
}
